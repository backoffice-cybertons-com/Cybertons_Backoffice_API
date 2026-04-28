-- ============================================================
-- FIX: fn_pay_rec INSERT validation
-- The function was blocking inserts because:
--   1. p_jv_code was validated as required -> but jv_code is optional (g_code is the link)
--   2. Tab_jv check ran BEFORE jv records existed in the same transaction
-- Run this in pgAdmin on your company database.
-- ============================================================

CREATE OR REPLACE FUNCTION public.fn_pay_rec(
    p_action text,
    p_code bigint DEFAULT NULL::bigint,
    p_company_id bigint DEFAULT NULL::bigint,
    p_date date DEFAULT NULL::date,
    p_inv_code bigint DEFAULT NULL::bigint,
    p_jv_code bigint DEFAULT NULL::bigint,
    p_amount numeric DEFAULT NULL::numeric,
    p_disc text DEFAULT NULL::text,
    p_party_id bigint DEFAULT NULL::bigint,
    p_account_id bigint DEFAULT NULL::bigint,
    p_discount_amount numeric DEFAULT 0,
    p_vch_type character varying DEFAULT NULL::character varying,
    p_g_code character varying DEFAULT NULL::character varying,
    p_year_code integer DEFAULT NULL::integer,
    p_branch_code integer DEFAULT NULL::integer,
    p_user_code integer DEFAULT NULL::integer,
    p_created_by character varying DEFAULT NULL::character varying,
    p_deleted_by character varying DEFAULT NULL::character varying,
    p_delete_flag boolean DEFAULT false
)
RETURNS SETOF tab_pay_rec
LANGUAGE plpgsql
COST 100
VOLATILE PARALLEL UNSAFE
ROWS 1000
AS $BODY$
BEGIN

    -- ============================
    -- INSERT
    -- ============================
    IF UPPER(p_action) = 'INSERT' THEN

        -- Basic validation only (removed strict jv_code / inv_code foreign-key checks
        -- because they are inserted within the same transaction and not yet visible)
        IF p_amount IS NULL OR p_amount <= 0 THEN
            RAISE EXCEPTION 'Amount must be greater than 0';
        END IF;

        IF p_vch_type IS NULL OR UPPER(p_vch_type) NOT IN ('PAYMENT', 'RECEIPT') THEN
            RAISE EXCEPTION 'Invalid voucher type: %', p_vch_type;
        END IF;

        RETURN QUERY
        INSERT INTO public.tab_pay_rec (
            date,
            inv_code,
            jv_code,
            amount,
            disc,
            party_id,
            account_id,
            discount_amount,
            g_code,
            company_id,
            year_code,
            branch_code,
            user_code,
            created_by,
            delete_flag,
            vch_type
        )
        VALUES (
            COALESCE(p_date, CURRENT_DATE),
            p_inv_code,
            p_jv_code,
            p_amount,
            p_disc,
            p_party_id,
            p_account_id,
            COALESCE(p_discount_amount, 0),
            p_g_code,
            p_company_id,
            p_year_code,
            p_branch_code,
            p_user_code,
            p_created_by,
            FALSE,
            p_vch_type
        )
        RETURNING *;

    -- ============================
    -- UPDATE
    -- ============================
    ELSIF UPPER(p_action) = 'UPDATE' THEN

        RETURN QUERY
        UPDATE public.tab_pay_rec
        SET
            date            = COALESCE(p_date, date),
            inv_code        = COALESCE(p_inv_code, inv_code),
            jv_code         = COALESCE(p_jv_code, jv_code),
            amount          = COALESCE(p_amount, amount),
            disc            = COALESCE(p_disc, disc),
            party_id        = COALESCE(p_party_id, party_id),
            account_id      = COALESCE(p_account_id, account_id),
            discount_amount = COALESCE(p_discount_amount, discount_amount),
            g_code          = COALESCE(p_g_code, g_code),
            year_code       = COALESCE(p_year_code, year_code),
            branch_code     = COALESCE(p_branch_code, branch_code),
            user_code       = COALESCE(p_user_code, user_code),
            vch_type        = COALESCE(p_vch_type, vch_type)
        WHERE code = p_code
          AND company_id = p_company_id
          AND delete_flag = FALSE
        RETURNING *;

    -- ============================
    -- DELETE (SOFT DELETE)
    -- ============================
    ELSIF UPPER(p_action) = 'DELETE' THEN

        RETURN QUERY
        UPDATE public.tab_pay_rec
        SET
            delete_flag = TRUE,
            deleted_by  = p_deleted_by,
            deleted_on  = CURRENT_TIMESTAMP
        WHERE code = p_code
          AND company_id = p_company_id
          AND delete_flag = FALSE
        RETURNING *;

    -- ============================
    -- GET BY ID
    -- ============================
    ELSIF UPPER(p_action) = 'GETBYID' THEN

        RETURN QUERY
        SELECT *
        FROM public.tab_pay_rec
        WHERE code = p_code
          AND company_id = p_company_id
          AND delete_flag = FALSE;

    -- ============================
    -- GET ALL
    -- ============================
    ELSIF UPPER(p_action) = 'GETALL' THEN

        RETURN QUERY
        SELECT *
        FROM public.tab_pay_rec
        WHERE company_id = p_company_id
          AND delete_flag = FALSE
          AND (p_vch_type IS NULL OR vch_type = p_vch_type)
        ORDER BY code DESC;

    ELSE
        RAISE EXCEPTION 'Invalid action: %', p_action;
    END IF;

END;
$BODY$;

ALTER FUNCTION public.fn_pay_rec(text, bigint, bigint, date, bigint, bigint, numeric, text, bigint, bigint, numeric, character varying, character varying, integer, integer, integer, character varying, character varying, boolean)
    OWNER TO postgres;
