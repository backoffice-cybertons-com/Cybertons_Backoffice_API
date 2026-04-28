-- UPDATE SCRIPT: Ledger Report Particulars Enhancement
-- Description: Updates fn_ledger_report_v2 to show the offsetting account name 
--              (e.g., "To Sales Revenue") instead of just the description.

-- Cleanup old signatures
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
    -- 1. Calculate History Balance (Before Start Date)
    -----------------------------------------
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
    SELECT COALESCE((
        SELECT 
            CASE 
                WHEN UPPER(TRIM(opening_balance_type)) IN ('DR', 'DEBIT', 'D', 'RECEIVABLE', 'REC', 'DEBT') THEN ABS(COALESCE(opening_balance, 0))::NUMERIC
                WHEN UPPER(TRIM(opening_balance_type)) IN ('CR', 'CREDIT', 'C', 'PAYABLE', 'PAY', 'CRED') THEN -ABS(COALESCE(opening_balance, 0))::NUMERIC
                ELSE 0 
            END
        FROM tab_party_master
        WHERE party_id = p_account_code 
        LIMIT 1
    ), 0.0) INTO v_party_opening;

    v_opening_balance := v_opening_balance + v_party_opening;

    -----------------------------------------
    -- 3. Return Unified Result Set
    -----------------------------------------
    RETURN QUERY
    WITH ledger_rows AS (
        -- [A] Opening Balance Row
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
        
        -- [B] Transaction Rows with Account Name Resolution
        SELECT 
            j.jv_date::DATE,
            CASE 
                WHEN j.v_type IS NOT NULL AND UPPER(j.v_type) <> 'JV' THEN j.v_type
                WHEN j.description LIKE '% Output - Inv %' THEN 'Sales'
                WHEN j.description LIKE '% Input - Bill %' THEN 'Purchase'
                ELSE COALESCE(j.v_type, 'JV')
            END::TEXT,
            CASE 
                WHEN UPPER(j.v_type) IN ('PAYMENT', 'RECEIPT') THEN j.jv_code::TEXT 
                ELSE COALESCE(j.v_code, 0)::TEXT 
            END,
            -- Resolve Particulars name: Show ONLY the offsetting account on top, description below (No prefixes)
            (CASE 
                WHEN j.dr_account_code = p_account_code THEN
                    COALESCE(cr_acc.name, 'Unknown Account') || 
                    CASE WHEN COALESCE(j.description, '') <> '' THEN CHR(10) || j.description ELSE '' END
                WHEN j.cr_account_code = p_account_code THEN
                    COALESCE(dr_acc.name, 'Unknown Account') || 
                    CASE WHEN COALESCE(j.description, '') <> '' THEN CHR(10) || j.description ELSE '' END
                ELSE
                    COALESCE(j.description, '')
            END)::TEXT,
            CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END,
            CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END,
            CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE -j.amount::NUMERIC END,
            1,
            j.jv_code
        FROM tab_jv j
        -- Lateral Joins to resolve names from both tables efficiently
        LEFT JOIN LATERAL (
            SELECT name FROM (
                SELECT display_name as name, 1 as priority FROM tab_party_master WHERE party_id = j.dr_account_code
                UNION ALL
                SELECT account_name as name, 2 as priority FROM tab_account_master WHERE account_id = j.dr_account_code
            ) n ORDER BY priority LIMIT 1
        ) dr_acc ON TRUE
        LEFT JOIN LATERAL (
            SELECT name FROM (
                SELECT display_name as name, 1 as priority FROM tab_party_master WHERE party_id = j.cr_account_code
                UNION ALL
                SELECT account_name as name, 2 as priority FROM tab_account_master WHERE account_id = j.cr_account_code
            ) n ORDER BY priority LIMIT 1
        ) cr_acc ON TRUE
        WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
          AND j.jv_date::DATE BETWEEN p_from_date AND p_to_date
          AND (j.delete_flag IS NOT TRUE)
    )
    SELECT 
        le.d, le.t, le.c, le.ds, le.dr, le.cr,
        SUM(le.amt) OVER (ORDER BY le.d, le.s, le.id),
        le.id
    FROM ledger_rows le
    ORDER BY le.d, le.s, le.id;
END;
$$ LANGUAGE plpgsql;
