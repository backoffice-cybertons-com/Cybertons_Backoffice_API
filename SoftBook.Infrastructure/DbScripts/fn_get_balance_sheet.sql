DROP FUNCTION IF EXISTS public.fn_get_balance_sheet(INTEGER, DATE);

CREATE OR REPLACE FUNCTION public.fn_get_balance_sheet(p_company_id INTEGER, p_as_of_date DATE)
RETURNS TABLE(
    report_side TEXT,
    row_type TEXT,
    group_name TEXT,
    ledger_id BIGINT,
    ledger_name TEXT,
    amount NUMERIC
) AS $$
BEGIN
    RETURN QUERY
    WITH params AS (
        SELECT
            p_company_id AS company_id,
            p_as_of_date AS as_of_date
    ),
    ledger_master AS (
        SELECT
            p.party_id::BIGINT AS ledger_id,
            COALESCE(
                NULLIF(TRIM(p.display_name), ''),
                NULLIF(TRIM(p.company_name), ''),
                p.party_id::TEXT
            ) AS ledger_name,
            pt.party_type_id,
            UPPER(pt.account_nature) AS account_nature,
            UPPER(pt.report_type) AS report_type,
            pt.report_group,
            pt.normal_balance,
            COALESCE(pt.is_dynamic_nature, FALSE) AS is_dynamic_nature,
            CASE
                WHEN UPPER(TRIM(COALESCE(p.opening_balance_type, ''))) IN ('DR','DEBIT','D','RECEIVABLE','REC','DEBT')
                    THEN ABS(COALESCE(p.opening_balance, 0))::NUMERIC
                WHEN UPPER(TRIM(COALESCE(p.opening_balance_type, ''))) IN ('CR','CREDIT','C','PAYABLE','PAY','CRED')
                    THEN -ABS(COALESCE(p.opening_balance, 0))::NUMERIC
                ELSE 0::NUMERIC
            END AS opening_signed
        FROM public.tab_party_master p
        LEFT JOIN public.tab_party_type_master pt
            ON p.party_type_id = pt.party_type_id
        CROSS JOIN params prm
        WHERE p.company_id = prm.company_id
    ),
    jv_flat AS (
        SELECT j.dr_account_code::BIGINT AS ledger_id, j.amount::NUMERIC AS signed_amount
        FROM public.tab_jv j
        CROSS JOIN params prm
        WHERE j.jv_comp_code = prm.company_id
          AND j.jv_date < prm.as_of_date + INTERVAL '1 day'
          AND COALESCE(j.delete_flag, FALSE) = FALSE
        UNION ALL
        SELECT j.cr_account_code::BIGINT AS ledger_id, -j.amount::NUMERIC AS signed_amount
        FROM public.tab_jv j
        CROSS JOIN params prm
        WHERE j.jv_comp_code = prm.company_id
          AND j.jv_date < prm.as_of_date + INTERVAL '1 day'
          AND COALESCE(j.delete_flag, FALSE) = FALSE
    ),
    ledger_balances AS (
        SELECT
            lm.ledger_id,
            lm.ledger_name,
            lm.account_nature,
            lm.report_type,
            lm.report_group,
            lm.normal_balance,
            lm.is_dynamic_nature,
            lm.opening_signed + COALESCE(SUM(jf.signed_amount), 0) AS closing_signed
        FROM ledger_master lm
        LEFT JOIN jv_flat jf ON jf.ledger_id = lm.ledger_id
        GROUP BY
            lm.ledger_id,
            lm.ledger_name,
            lm.account_nature,
            lm.report_type,
            lm.report_group,
            lm.normal_balance,
            lm.is_dynamic_nature,
            lm.opening_signed
    ),
    profit_loss_calc AS (
        SELECT COALESCE(SUM(lb.closing_signed), 0) AS pl_signed
        FROM ledger_balances lb
        WHERE lb.report_type = 'PROFIT_LOSS'
    ),
    balance_sheet_rows AS (
        SELECT
            CASE
                WHEN lb.account_nature = 'ASSET' AND lb.closing_signed >= 0 THEN 'ASSETS'
                WHEN lb.account_nature = 'ASSET' AND lb.closing_signed < 0 THEN 'LIABILITIES'
                WHEN lb.account_nature = 'LIABILITY' AND lb.closing_signed <= 0 THEN 'LIABILITIES'
                WHEN lb.account_nature = 'LIABILITY' AND lb.closing_signed > 0 THEN 'ASSETS'
                WHEN lb.is_dynamic_nature = TRUE AND lb.closing_signed < 0 THEN 'LIABILITIES'
                WHEN lb.is_dynamic_nature = TRUE AND lb.closing_signed > 0 THEN 'ASSETS'
            END AS report_side,
            lb.report_group AS group_name,
            lb.ledger_id,
            lb.ledger_name,
            ABS(lb.closing_signed) AS final_amount
        FROM ledger_balances lb
        WHERE lb.report_type = 'BALANCE_SHEET'
          AND ROUND(lb.closing_signed, 2) <> 0
    ),
    profit_loss_balance_row AS (
        SELECT
            CASE WHEN pc.pl_signed < 0 THEN 'LIABILITIES' ELSE 'ASSETS' END AS report_side,
            'Profit & Loss' AS group_name,
            NULL::BIGINT AS ledger_id,
            CASE WHEN pc.pl_signed < 0 THEN 'Current Profit' ELSE 'Current Loss' END AS ledger_name,
            ABS(pc.pl_signed) AS final_amount
        FROM profit_loss_calc pc
        WHERE ROUND(pc.pl_signed, 2) <> 0
    ),
    all_balance_rows AS (
        SELECT bs.report_side, bs.group_name, bs.ledger_id, bs.ledger_name, bs.final_amount FROM balance_sheet_rows bs
        UNION ALL
        SELECT pl.report_side, pl.group_name, pl.ledger_id, pl.ledger_name, pl.final_amount FROM profit_loss_balance_row pl
    ),
    report_rows AS (
        SELECT abr.report_side, 'DETAIL' AS row_type, abr.group_name, abr.ledger_id, abr.ledger_name, abr.final_amount AS amount
        FROM all_balance_rows abr
        UNION ALL
        SELECT abr.report_side, 'GROUP_TOTAL' AS row_type, abr.group_name, NULL::BIGINT AS ledger_id, abr.group_name || ' Total' AS ledger_name, SUM(abr.final_amount) AS amount
        FROM all_balance_rows abr
        GROUP BY abr.report_side, abr.group_name
        UNION ALL
        SELECT abr.report_side, 'GRAND_TOTAL' AS row_type, 'TOTAL' AS group_name, NULL::BIGINT AS ledger_id, abr.report_side || ' TOTAL' AS ledger_name, SUM(abr.final_amount) AS amount
        FROM all_balance_rows abr
        GROUP BY abr.report_side
    )
    SELECT
        rr.report_side::TEXT,
        rr.row_type::TEXT,
        rr.group_name::TEXT,
        rr.ledger_id::BIGINT,
        rr.ledger_name::TEXT,
        ROUND(rr.amount, 2)::NUMERIC AS amount
    FROM report_rows rr
    ORDER BY
        CASE WHEN rr.report_side = 'LIABILITIES' THEN 1 ELSE 2 END,
        rr.group_name,
        CASE rr.row_type WHEN 'DETAIL' THEN 1 WHEN 'GROUP_TOTAL' THEN 2 ELSE 3 END,
        rr.ledger_name;
END;
$$ LANGUAGE plpgsql;
