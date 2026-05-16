-- Fix for Ledger Datewise Reports (Summary and Detailed)
-- Updates the functions to accept 4 parameters to match the C# Backend ReportRepository:
-- p_company_id, p_account_code, p_from_date, p_to_date

-- 1. Detailed Ledger Report (fn_ledger_report_v2)
DROP FUNCTION IF EXISTS fn_ledger_report_v2(INT, TIMESTAMP, TIMESTAMP);
DROP FUNCTION IF EXISTS fn_ledger_report_v2(INT, DATE, DATE);
DROP FUNCTION IF EXISTS fn_ledger_report_v2(INT, INT, DATE, DATE);

CREATE OR REPLACE FUNCTION fn_ledger_report_v2(
    p_company_id INT,
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
    SELECT COALESCE(SUM(
        CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END - 
        CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END
    ), 0.0)
    INTO v_opening_balance
    FROM tab_jv j
    WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
      AND j.jv_date::DATE < p_from_date
      AND (j.delete_flag IS NOT TRUE)
      AND j.jv_comp_code = p_company_id;

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
        
        -- [B] Transaction Rows
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
            1,
            j.jv_code
        FROM tab_jv j
        WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
          AND j.jv_date::DATE BETWEEN p_from_date AND p_to_date
          AND (j.delete_flag IS NOT TRUE)
          AND j.jv_comp_code = p_company_id
    )
    SELECT 
        le.d, le.t, le.c, le.ds, le.dr, le.cr,
        SUM(le.amt) OVER (ORDER BY le.d, le.s, le.id),
        le.id
    FROM ledger_rows le
    ORDER BY le.d, le.s, le.id;
END;
$$ LANGUAGE plpgsql;

-- 2. Summary Ledger Datewise Report (fn_ledger_summary_datewise)
DROP FUNCTION IF EXISTS public.fn_ledger_summary_datewise(integer, date, date);
DROP FUNCTION IF EXISTS public.fn_ledger_summary_datewise(integer, integer, date, date);

CREATE OR REPLACE FUNCTION public.fn_ledger_summary_datewise(
    p_company_id integer,
    p_account_code integer,
    p_from_date date,
    p_to_date date
)
RETURNS TABLE(
    res_date date,
    res_particular text,
    res_debit numeric,
    res_credit numeric,
    res_balance numeric
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_opening_balance NUMERIC := 0.0;
    v_party_opening NUMERIC := 0.0;
BEGIN

    -----------------------------------------
    -- 1. Calculate Opening Balance from History
    -----------------------------------------
    SELECT COALESCE(SUM(
        CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END -
        CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END
    ), 0.0)
    INTO v_opening_balance
    FROM tab_jv j
    WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
      AND j.jv_date::DATE < p_from_date
      AND (j.delete_flag IS NOT TRUE)
      AND j.jv_comp_code = p_company_id;

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
           OR accounts_receivable_account = p_account_code 
           OR accounts_payable_account = p_account_code
        LIMIT 1
    ), 0.0) INTO v_party_opening;

    v_opening_balance := v_opening_balance + v_party_opening;

    -----------------------------------------
    -- 3. Return Summary Data Set
    -----------------------------------------
    RETURN QUERY
    WITH DatewiseData AS (
        -- [A] Opening Balance Row
        SELECT
            p_from_date AS trans_date,
            'Opening Balance'::TEXT AS particular,
            CASE WHEN v_opening_balance > 0 THEN v_opening_balance ELSE 0.0 END AS dr_amt,
            CASE WHEN v_opening_balance < 0 THEN ABS(v_opening_balance) ELSE 0.0 END AS cr_amt,
            v_opening_balance AS net_amt,
            0 AS sort_order
            
        UNION ALL
        
        -- [B] Transaction Rows (Grouped by Date)
        SELECT
            j.jv_date::DATE AS trans_date,
            'Transactions'::TEXT AS particular,
            SUM(CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END) AS dr_amt,
            SUM(CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END) AS cr_amt,
            SUM(CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE -j.amount::NUMERIC END) AS net_amt,
            1 AS sort_order
        FROM tab_jv j
        WHERE (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
          AND j.jv_date::DATE BETWEEN p_from_date AND p_to_date
          AND (j.delete_flag IS NOT TRUE)
          AND j.jv_comp_code = p_company_id
        GROUP BY j.jv_date::DATE
    )
    SELECT
        d.trans_date,
        d.particular,
        d.dr_amt,
        d.cr_amt,
        SUM(d.net_amt) OVER (ORDER BY d.trans_date, d.sort_order) AS res_balance
    FROM DatewiseData d
    ORDER BY d.trans_date, d.sort_order;
    
END;
$$;
