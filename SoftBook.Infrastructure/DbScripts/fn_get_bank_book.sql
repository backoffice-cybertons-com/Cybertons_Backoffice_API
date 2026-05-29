DROP FUNCTION IF EXISTS public.fn_get_bank_book(INTEGER, DATE, DATE);

CREATE OR REPLACE FUNCTION public.fn_get_bank_book(p_company_id INTEGER, p_from_date DATE, p_to_date DATE)
RETURNS TABLE(
    transaction_date DATE,
    voucher_no BIGINT,
    voucher_type TEXT,
    particulars TEXT,
    receipt_amount NUMERIC,
    payment_amount NUMERIC,
    balance NUMERIC,
    bank_name TEXT
) AS $$
DECLARE
    v_opening_balance NUMERIC := 0;
BEGIN
    -- 1. Calculate Opening Balance for all Bank accounts
    WITH bank_accounts AS (
        SELECT p.party_id
        FROM public.tab_party_master p
        JOIN public.tab_party_type_master pt ON p.party_type_id = pt.party_type_id
        WHERE p.company_id = p_company_id
          AND (pt.party_type_code ILIKE '%BANK%' OR pt.party_type_name ILIKE '%BANK%')
    ),
    master_opening AS (
        SELECT SUM(
            CASE 
                WHEN UPPER(TRIM(COALESCE(p.opening_balance_type, ''))) IN ('DR','DEBIT','D','RECEIVABLE','REC','DEBT') THEN ABS(COALESCE(p.opening_balance, 0))
                WHEN UPPER(TRIM(COALESCE(p.opening_balance_type, ''))) IN ('CR','CREDIT','C','PAYABLE','PAY','CRED') THEN -ABS(COALESCE(p.opening_balance, 0))
                ELSE 0
            END
        ) AS amt
        FROM public.tab_party_master p
        JOIN bank_accounts ba ON p.party_id = ba.party_id
    ),
    prior_transactions AS (
        SELECT SUM(
            CASE WHEN j.dr_account_code IN (SELECT party_id FROM bank_accounts) THEN j.amount ELSE 0 END
            -
            CASE WHEN j.cr_account_code IN (SELECT party_id FROM bank_accounts) THEN j.amount ELSE 0 END
        ) AS amt
        FROM public.tab_jv j
        WHERE j.jv_comp_code = p_company_id
          AND j.jv_date < p_from_date
          AND COALESCE(j.delete_flag, FALSE) = FALSE
          AND (j.dr_account_code IN (SELECT party_id FROM bank_accounts) OR j.cr_account_code IN (SELECT party_id FROM bank_accounts))
    )
    SELECT COALESCE((SELECT amt FROM master_opening), 0) + COALESCE((SELECT amt FROM prior_transactions), 0)
    INTO v_opening_balance;

    -- Return the Opening Balance Row
    RETURN QUERY SELECT 
        (p_from_date - INTERVAL '1 day')::DATE AS transaction_date,
        NULL::BIGINT AS voucher_no,
        'Opening'::TEXT AS voucher_type,
        'Opening Balance'::TEXT AS particulars,
        (CASE WHEN v_opening_balance >= 0 THEN v_opening_balance ELSE 0::NUMERIC END)::NUMERIC AS receipt_amount,
        (CASE WHEN v_opening_balance < 0 THEN ABS(v_opening_balance) ELSE 0::NUMERIC END)::NUMERIC AS payment_amount,
        v_opening_balance::NUMERIC AS balance,
        NULL::TEXT AS bank_name;

    -- Return the Transactions
    RETURN QUERY 
    WITH bank_accounts AS (
        SELECT p.party_id
        FROM public.tab_party_master p
        JOIN public.tab_party_type_master pt ON p.party_type_id = pt.party_type_id
        WHERE p.company_id = p_company_id
          AND (pt.party_type_code ILIKE '%BANK%' OR pt.party_type_name ILIKE '%BANK%')
    ),
    txns AS (
        SELECT 
            j.jv_date::DATE AS transaction_date,
            j.jv_code::BIGINT AS voucher_no,
            j.v_type::TEXT AS voucher_type,
            CASE 
                WHEN j.dr_account_code IN (SELECT party_id FROM bank_accounts) THEN 
                    (SELECT COALESCE(NULLIF(TRIM(display_name), ''), NULLIF(TRIM(company_name), ''), party_id::TEXT) FROM public.tab_party_master WHERE party_id = j.cr_account_code)
                ELSE 
                    (SELECT COALESCE(NULLIF(TRIM(display_name), ''), NULLIF(TRIM(company_name), ''), party_id::TEXT) FROM public.tab_party_master WHERE party_id = j.dr_account_code)
            END AS particulars,
            CASE WHEN j.dr_account_code IN (SELECT party_id FROM bank_accounts) THEN j.amount::NUMERIC ELSE 0::NUMERIC END AS receipt_amount,
            CASE WHEN j.cr_account_code IN (SELECT party_id FROM bank_accounts) THEN j.amount::NUMERIC ELSE 0::NUMERIC END AS payment_amount,
            (CASE WHEN j.dr_account_code IN (SELECT party_id FROM bank_accounts) THEN j.amount::NUMERIC ELSE 0::NUMERIC END 
             - CASE WHEN j.cr_account_code IN (SELECT party_id FROM bank_accounts) THEN j.amount::NUMERIC ELSE 0::NUMERIC END) AS net_amount,
            CASE 
                WHEN j.dr_account_code IN (SELECT party_id FROM bank_accounts) THEN 
                    (SELECT COALESCE(NULLIF(TRIM(display_name), ''), NULLIF(TRIM(company_name), ''), party_id::TEXT) FROM public.tab_party_master WHERE party_id = j.dr_account_code)
                ELSE 
                    (SELECT COALESCE(NULLIF(TRIM(display_name), ''), NULLIF(TRIM(company_name), ''), party_id::TEXT) FROM public.tab_party_master WHERE party_id = j.cr_account_code)
            END AS bank_name
        FROM public.tab_jv j
        WHERE j.jv_comp_code = p_company_id
          AND j.jv_date >= p_from_date
          AND j.jv_date < p_to_date + INTERVAL '1 day'
          AND COALESCE(j.delete_flag, FALSE) = FALSE
          AND (j.dr_account_code IN (SELECT party_id FROM bank_accounts) OR j.cr_account_code IN (SELECT party_id FROM bank_accounts))
        ORDER BY j.jv_date, j.jv_code
    )
    SELECT 
        t.transaction_date::DATE,
        t.voucher_no::BIGINT,
        t.voucher_type::TEXT,
        t.particulars::TEXT,
        t.receipt_amount::NUMERIC,
        t.payment_amount::NUMERIC,
        (SUM(t.net_amount) OVER (ORDER BY t.transaction_date, t.voucher_no ROWS UNBOUNDED PRECEDING) + v_opening_balance)::NUMERIC AS balance,
        t.bank_name::TEXT AS bank_name
    FROM txns t;
END;
$$ LANGUAGE plpgsql;
