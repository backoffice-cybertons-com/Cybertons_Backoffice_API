-- UPDATE SCRIPT: Ledger Report Opening Balance Integration
-- Target: PostgreSQL
-- Description: Updates fn_ledger_report_v2 to include the Opening Balance from tab_party_master.

-- Cleanup old signatures just in case
DROP FUNCTION IF EXISTS fn_ledger_report_v2(INT, TIMESTAMP, TIMESTAMP);
DROP FUNCTION IF EXISTS fn_ledger_report_v2(INT, DATE, DATE);

CREATE OR REPLACE FUNCTION fn_ledger_report_v2(
    p_account_code INT,
    p_from_date DATE,
    p_to_date DATE
)
RETURNS TABLE
(
    res_jv_date DATE,
    res_v_type TEXT,
    res_v_code TEXT,
    res_description TEXT,
    res_debit NUMERIC,
    res_credit NUMERIC,
    res_balance NUMERIC,
    res_id BIGINT
)
AS
$$
DECLARE
    v_opening_balance NUMERIC := 0.0;
    v_party_opening NUMERIC := 0.0;
BEGIN
    -----------------------------------------
    -- 1. Calculate Opening Balance from History
    -----------------------------------------
    -- Sum of (debit - credit) for the account BEFORE the p_from_date
    SELECT COALESCE(SUM(
        CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END - 
        CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END
    ), 0.0)
    INTO v_opening_balance
    FROM tab_jv j
    WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
      AND j.jv_date::DATE < p_from_date
      AND (j.delete_flag IS NOT TRUE);

    -----------------------------------------
    -- 2. Add Static Opening Balance from Party Master
    -----------------------------------------
    -- We lookup by party_id (New Architecture) OR linked account codes (Old Architecture)
    SELECT COALESCE((
        SELECT 
            CASE 
                WHEN UPPER(TRIM(opening_balance_type)) IN ('DR', 'DEBIT', 'D', 'RECEIVABLE', 'REC', 'DEBT') THEN ABS(COALESCE(opening_balance, 0))::NUMERIC
                WHEN UPPER(TRIM(opening_balance_type)) IN ('CR', 'CREDIT', 'C', 'PAYABLE', 'PAY', 'CRED') THEN -ABS(COALESCE(opening_balance, 0))::NUMERIC
                ELSE 0 
            END
        FROM tab_party_master
        WHERE party_id = p_account_code 
           OR accounts_receivable_account = p_account_code 
           OR accounts_payable_account = p_account_code
        LIMIT 1
    ), 0.0) INTO v_party_opening;

    v_opening_balance := v_opening_balance + v_party_opening;

    -----------------------------------------
    -- 3. Return Unified Data Set
    -----------------------------------------
    RETURN QUERY
    WITH ledger_rows AS (
        -- [A] Opening Balance Row (Synthetic)
        SELECT 
            p_from_date::DATE as d,
            'Opening Balance'::TEXT as t,
            ''::TEXT as c,
            'Opening Balance b/f'::TEXT as ds,
            CASE WHEN v_opening_balance >= 0 THEN v_opening_balance ELSE 0.0 END as dr,
            CASE WHEN v_opening_balance < 0 THEN ABS(v_opening_balance) ELSE 0.0 END as cr,
            v_opening_balance as amt,
            0 as s,
            0::BIGINT as id
            
        UNION ALL
        
        -- [B] Transaction Rows (Actual Records from tab_jv)
        SELECT 
            j.jv_date::DATE,
            COALESCE(j.v_type, 'JV')::TEXT,
            -- For Payment/Receipt, use jv_code as the ID. For others, use v_code.
            CASE 
                WHEN UPPER(j.v_type) IN ('PAYMENT', 'RECEIPT') THEN j.jv_code::TEXT 
                ELSE COALESCE(j.v_code, 0)::TEXT 
            END,
            COALESCE(j.description, '')::TEXT,
            CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END,
            CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END,
            CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE -j.amount::NUMERIC END,
            1,
            j.jv_code
        FROM tab_jv j
        WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
          AND j.jv_date::DATE BETWEEN p_from_date AND p_to_date
          AND (j.delete_flag IS NOT TRUE)
    )
    SELECT 
        le.d, le.t, le.c, le.ds, le.dr, le.cr,
        -- Calculate Running Balance using Window Function
        SUM(le.amt) OVER (ORDER BY le.d, le.s, le.id),
        le.id
    FROM ledger_rows le
    ORDER BY le.d, le.s, le.id;
END;
$$ LANGUAGE plpgsql;
