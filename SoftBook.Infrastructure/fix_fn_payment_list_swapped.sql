-- ============================================================
-- FIXED fn_payment_list with Swapped Account Display
-- For PAYMENT: 
--    Main Account column = Party (e.g. MMM)
--    Party Account column = Bank/Cash (e.g. CASH01)
-- For RECEIPT:
--    Main Account column = Bank/Cash (e.g. CASH01)
--    Party Account column = Party (e.g. MMM)
-- ============================================================

CREATE OR REPLACE FUNCTION public.fn_payment_list(
    p_company_id INT,
    p_page_no    INT  DEFAULT 1,
    p_page_size  INT  DEFAULT 50,
    p_v_type     VARCHAR DEFAULT NULL,
    p_party_id   INT DEFAULT NULL
)
RETURNS TABLE (
    sr_no         BIGINT,
    jv_code       BIGINT,
    inv_no        VARCHAR,
    bill_no       INT,
    jv_date       DATE,
    due_date      DATE,
    v_type        VARCHAR,
    main_account  BIGINT,
    trn_account   BIGINT,
    description   TEXT,
    party_name    VARCHAR,
    account_name  VARCHAR,
    amount        NUMERIC,
    sp_amount     NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    WITH grouped AS (
        SELECT
            pr.g_code,
            UPPER(MAX(pr.vch_type))             AS vch_type,
            MAX(pr.date)                         AS vch_date,
            MAX(pr.party_id)                     AS party_id,
            MAX(pr.account_id)                   AS account_id,
            SUM(pr.amount)                       AS total_amount,
            MAX(pr.code)                         AS max_code
        FROM public.tab_pay_rec pr
        WHERE pr.company_id  = p_company_id
          AND pr.delete_flag = FALSE
          AND (p_v_type  IS NULL OR UPPER(pr.vch_type) = UPPER(p_v_type))
          AND (p_party_id IS NULL OR pr.party_id = p_party_id)
        GROUP BY pr.g_code
    )
    SELECT
        CAST(ROW_NUMBER() OVER (ORDER BY g.vch_date DESC, g.max_code DESC) AS BIGINT)  AS sr_no,
        CAST(g.max_code        AS BIGINT)   AS jv_code,
        CAST(g.g_code          AS VARCHAR)  AS inv_no,
        CAST(0                 AS INT)      AS bill_no,
        CAST(g.vch_date        AS DATE)     AS jv_date,
        CAST(g.vch_date        AS DATE)     AS due_date,
        CAST(g.vch_type        AS VARCHAR)  AS v_type,
        
        -- Internal IDs (Swapped logic)
        CAST(CASE WHEN g.vch_type = 'PAYMENT' THEN g.party_id ELSE g.account_id END AS BIGINT) AS main_account,
        CAST(CASE WHEN g.vch_type = 'PAYMENT' THEN g.account_id ELSE g.party_id END AS BIGINT) AS trn_account,
        
        CAST(NULL              AS TEXT)     AS description,
        
        -- Party Account Column (Mapped to party_name in frontend)
        CAST(CASE WHEN g.vch_type = 'PAYMENT' THEN a.display_name ELSE p.display_name END AS VARCHAR) AS party_name,
        
        -- Main Account Column (Mapped to account_name in frontend)
        CAST(CASE WHEN g.vch_type = 'PAYMENT' THEN p.display_name ELSE a.display_name END AS VARCHAR) AS account_name,
        
        CAST(g.total_amount    AS NUMERIC)  AS amount,
        CAST(0.0               AS NUMERIC)  AS sp_amount
    FROM grouped g
    LEFT JOIN public.tab_party_master p ON p.party_id = g.party_id
    LEFT JOIN public.tab_party_master a ON a.party_id = g.account_id
    ORDER BY g.vch_date DESC, g.max_code DESC
    LIMIT  p_page_size
    OFFSET (p_page_no - 1) * p_page_size;
END;
$$;

ALTER FUNCTION public.fn_payment_list(INT, INT, INT, VARCHAR, INT)
    OWNER TO postgres;
