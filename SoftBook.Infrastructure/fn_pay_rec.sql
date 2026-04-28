-- ============================================================
-- SoftBook: Payment/Receipt Table and Function
-- Run this script ONCE on your company database to create
-- the tab_pay_rec table and fn_pay_rec function.
-- ============================================================

-- 1. Create the table (if it doesn't exist)
CREATE TABLE IF NOT EXISTS tab_pay_rec (
    code          BIGSERIAL PRIMARY KEY,
    date          TIMESTAMPTZ,
    inv_code      BIGINT,
    jv_code       BIGINT,
    amount        NUMERIC(18, 2),
    disc          VARCHAR(200),
    created_by    VARCHAR(100),
    created_on    TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    user_code     INT,
    post_cheque   BIGINT,
    company_id    BIGINT NOT NULL,
    full_half     VARCHAR(10),
    year_code     INT,
    branch_code   INT,
    vch_type      VARCHAR(50),
    deleted_by    VARCHAR(100),
    deleted_on    TIMESTAMPTZ,
    delete_flag   BOOLEAN DEFAULT FALSE,
    party_id      BIGINT,
    account_id    BIGINT,
    discount_amount NUMERIC(18, 2) DEFAULT 0,
    g_code        VARCHAR(100)
);

-- Indexes for fast lookups
CREATE INDEX IF NOT EXISTS idx_pay_rec_company   ON tab_pay_rec(company_id);
CREATE INDEX IF NOT EXISTS idx_pay_rec_g_code    ON tab_pay_rec(g_code);
CREATE INDEX IF NOT EXISTS idx_pay_rec_party     ON tab_pay_rec(party_id);
CREATE INDEX IF NOT EXISTS idx_pay_rec_vch_type  ON tab_pay_rec(vch_type);


-- 2. Create (or replace) the fn_pay_rec function
--    The function handles: GETALL | GETBYID | INSERT | UPDATE | DELETE
DROP FUNCTION IF EXISTS fn_pay_rec(TEXT, BIGINT, VARCHAR);
DROP FUNCTION IF EXISTS fn_pay_rec(TEXT, BIGINT, BIGINT);
DROP FUNCTION IF EXISTS fn_pay_rec(TEXT, TIMESTAMPTZ, BIGINT, BIGINT, NUMERIC, VARCHAR, BIGINT, BIGINT, NUMERIC, VARCHAR, VARCHAR, BIGINT, INT, INT, INT, VARCHAR);

CREATE OR REPLACE FUNCTION fn_pay_rec(
    p_action         TEXT,
    p_date           TIMESTAMPTZ  DEFAULT NULL,
    p_inv_code       BIGINT       DEFAULT NULL,
    p_jv_code        BIGINT       DEFAULT NULL,
    p_amount         NUMERIC      DEFAULT NULL,
    p_disc           VARCHAR      DEFAULT NULL,
    p_party_id       BIGINT       DEFAULT NULL,
    p_account_id     BIGINT       DEFAULT NULL,
    p_discount_amount NUMERIC     DEFAULT 0,
    p_vch_type       VARCHAR      DEFAULT NULL,
    p_g_code         VARCHAR      DEFAULT NULL,
    p_company_id     BIGINT       DEFAULT NULL,
    p_year_code      INT          DEFAULT NULL,
    p_branch_code    INT          DEFAULT NULL,
    p_user_code      INT          DEFAULT NULL,
    p_created_by     VARCHAR      DEFAULT NULL,
    p_code           BIGINT       DEFAULT NULL,
    p_deleted_by     VARCHAR      DEFAULT NULL,
    p_user_id        INT          DEFAULT NULL
)
RETURNS SETOF tab_pay_rec
LANGUAGE plpgsql
AS $$
DECLARE
    v_tab_id INT;
BEGIN
    -- Determine Tab ID based on voucher type
    -- Receipt = 41, Payment = 42
    IF p_vch_type = 'RECEIPT' THEN
        v_tab_id := 41;
    ELSIF p_vch_type = 'PAYMENT' THEN
        v_tab_id := 42;
    ELSE
        v_tab_id := 41; -- Fallback to Receipt if unknown (usually it's one of these)
    END IF;

    -- Permission Validation
    IF p_action = 'INSERT' THEN
        PERFORM public.fn_validate_permission(p_user_id, p_company_id::INT, v_tab_id, 'ADD');
    ELSIF p_action = 'UPDATE' THEN
        PERFORM public.fn_validate_permission(p_user_id, p_company_id::INT, v_tab_id, 'EDIT');
    ELSIF p_action = 'DELETE' THEN
        PERFORM public.fn_validate_permission(p_user_id, p_company_id::INT, v_tab_id, 'DELETE');
    END IF;

    -- -------------------------------------------------------
    -- GETALL: used by list/grid views
    -- -------------------------------------------------------
    IF p_action = 'GETALL' THEN
        RETURN QUERY
        SELECT * FROM tab_pay_rec
        WHERE company_id = p_company_id
          AND delete_flag = FALSE
          AND (p_vch_type IS NULL OR vch_type = p_vch_type)
        ORDER BY code DESC;

    -- -------------------------------------------------------
    -- GETBYID: used by the edit form
    -- -------------------------------------------------------
    ELSIF p_action = 'GETBYID' THEN
        RETURN QUERY
        SELECT * FROM tab_pay_rec
        WHERE company_id = p_company_id
          AND code       = p_code
          AND delete_flag = FALSE;

    -- -------------------------------------------------------
    -- INSERT: create a new payment / receipt
    -- -------------------------------------------------------
    ELSIF p_action = 'INSERT' THEN
        RETURN QUERY
        INSERT INTO tab_pay_rec (
            date, inv_code, jv_code, amount, disc, party_id, account_id,
            discount_amount, vch_type, g_code, company_id, year_code,
            branch_code, user_code, created_by, created_on, delete_flag
        ) VALUES (
            p_date, p_inv_code, p_jv_code, p_amount, p_disc, p_party_id, p_account_id,
            COALESCE(p_discount_amount, 0), p_vch_type, p_g_code, p_company_id,
            p_year_code, p_branch_code, p_user_code, p_created_by, CURRENT_TIMESTAMP, FALSE
        )
        RETURNING *;

    -- -------------------------------------------------------
    -- UPDATE: edit an existing payment / receipt
    -- -------------------------------------------------------
    ELSIF p_action = 'UPDATE' THEN
        RETURN QUERY
        UPDATE tab_pay_rec SET
            date            = COALESCE(p_date, date),
            inv_code        = COALESCE(p_inv_code, inv_code),
            jv_code         = COALESCE(p_jv_code, jv_code),
            amount          = COALESCE(p_amount, amount),
            disc            = COALESCE(p_disc, disc),
            party_id        = COALESCE(p_party_id, party_id),
            account_id      = COALESCE(p_account_id, account_id),
            discount_amount = COALESCE(p_discount_amount, discount_amount),
            vch_type        = COALESCE(p_vch_type, vch_type),
            g_code          = COALESCE(p_g_code, g_code),
            year_code       = COALESCE(p_year_code, year_code),
            branch_code     = COALESCE(p_branch_code, branch_code),
            user_code       = COALESCE(p_user_code, user_code)
        WHERE code = p_code
          AND company_id = p_company_id
          AND delete_flag = FALSE
        RETURNING *;

    -- -------------------------------------------------------
    -- DELETE: soft-delete a single record by code
    -- -------------------------------------------------------
    ELSIF p_action = 'DELETE' THEN
        UPDATE tab_pay_rec SET
            delete_flag = TRUE,
            deleted_on  = CURRENT_TIMESTAMP,
            deleted_by  = p_deleted_by
        WHERE code = p_code
          AND company_id = p_company_id;
        -- Return empty set (soft delete)
        RETURN;

    END IF;
END;
$$;
