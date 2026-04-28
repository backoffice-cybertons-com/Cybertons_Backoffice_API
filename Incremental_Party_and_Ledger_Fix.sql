-- REFINED FIX: Accurate Opening Balance Integration
-- This version ensures the Party Master balance is correctly incorporated into the Running Total.

-- 1. Metadata Schema Fix (Prevents 'modified_on' save errors)
ALTER TABLE tab_party_master ADD COLUMN IF NOT EXISTS created_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE tab_party_master ADD COLUMN IF NOT EXISTS modified_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP;

-- 2. Update Ledger Report Logic
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
    v_opening_val NUMERIC := 0.0;
    v_party_bal NUMERIC := 0.0;
BEGIN
    -- [A] Calculate History Balance (Transactions before the start date)
    SELECT COALESCE(SUM(
        CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END - 
        CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END
    ), 0.0) INTO v_opening_val
    FROM tab_jv j
    WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
      AND j.jv_date::DATE < p_from_date
      AND (j.delete_flag IS NOT TRUE);

    -- [B] Fetch Static Opening Balance from tab_party_master
    -- Handles variations: Dr, Debit, D, Receivable, Rec, Pay, Cr, Credit, C, Payable
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
    ), 0.0) INTO v_party_bal;

    -- Combine the initial balance
    v_opening_val := v_opening_val + v_party_bal;

    -- [C] Return Unified Result Set with Accurate Running Total
    RETURN QUERY
    SELECT 
        q.final_date, q.final_type, q.final_code, q.final_desc, q.final_dr, q.final_cr,
        SUM(q.final_amt) OVER (ORDER BY q.final_date, q.sort_key, q.final_id),
        q.final_id
    FROM (
        -- Synthetic row for the Opening Balance b/f
        SELECT 
            p_from_date::DATE as final_date,
            'Opening Balance'::TEXT as final_type,
            ''::TEXT as final_code,
            'Opening Balance b/f'::TEXT as final_desc,
            CASE WHEN v_opening_val >= 0 THEN v_opening_val ELSE 0.0 END as final_dr,
            CASE WHEN v_opening_val < 0 THEN ABS(v_opening_val) ELSE 0.0 END as final_cr,
            v_opening_val as final_amt,
            0 as sort_key,
            0::BIGINT as final_id
            
        UNION ALL
        
        -- Transactional rows
        SELECT 
            j.jv_date::DATE,
            COALESCE(j.v_type, 'JV')::TEXT,
            CASE 
                WHEN UPPER(j.v_type) IN ('PAYMENT', 'RECEIPT') THEN j.jv_code::TEXT 
                ELSE COALESCE(j.v_code, 0)::TEXT 
            END,
            COALESCE(j.description, '')::TEXT,
            CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END,
            CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END,
            CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE -j.amount::NUMERIC END,
            1 as sort_key,
            j.jv_code
        FROM tab_jv j
        WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
          AND j.jv_date::DATE BETWEEN p_from_date AND p_to_date
          AND (j.delete_flag IS NOT TRUE)
    ) q;
END;
$$ LANGUAGE plpgsql;
