-- Ledger summary aggregated by date (Running Balance / Dual Ledger date-wise in UI).
-- Backend: ReportRepository.GetLedgerSummaryDatewiseAsync -> SELECT * FROM fn_ledger_summary_datewise(...)
-- Columns must match: res_date, res_particular, res_debit, res_credit, res_balance
-- UI (LedgerReport.js) expects res_particular: 'Opening Balance', per-day rows, and 'Total'.

DROP FUNCTION IF EXISTS fn_ledger_summary_datewise(INT, INT, DATE, DATE);
DROP FUNCTION IF EXISTS fn_ledger_summary_datewise(INTEGER, INTEGER, DATE, DATE);

CREATE OR REPLACE FUNCTION fn_ledger_summary_datewise(
    p_company_id INT,
    p_account_code INT,
    p_from_date DATE,
    p_to_date DATE
)
RETURNS TABLE (
    res_date DATE,
    res_particular TEXT,
    res_debit NUMERIC,
    res_credit NUMERIC,
    res_balance NUMERIC
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_hist NUMERIC := 0;
    v_party NUMERIC := 0;
    v_opening NUMERIC := 0;
BEGIN
    -- Net (Dr positive) from journals strictly before the period
    SELECT COALESCE(SUM(
        CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0 END
        - CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0 END
    ), 0)
    INTO v_hist
    FROM tab_jv j
    WHERE j.jv_comp_code = p_company_id
      AND (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
      AND j.jv_date::DATE < p_from_date
      AND (j.delete_flag IS NOT TRUE);

    -- Party master opening (same sign convention as fn_ledger_report_v2 scripts)
    SELECT COALESCE((
        SELECT
            CASE
                WHEN UPPER(TRIM(opening_balance_type)) IN ('DR', 'DEBIT', 'D', 'RECEIVABLE', 'REC', 'DEBT')
                    THEN ABS(COALESCE(opening_balance, 0))::NUMERIC
                WHEN UPPER(TRIM(opening_balance_type)) IN ('CR', 'CREDIT', 'C', 'PAYABLE', 'PAY', 'CRED')
                    THEN -ABS(COALESCE(opening_balance, 0))::NUMERIC
                ELSE 0::NUMERIC
            END
        FROM tab_party_master
        WHERE company_id = p_company_id
          AND (
              party_id = p_account_code
              OR accounts_receivable_account = p_account_code
              OR accounts_payable_account = p_account_code
          )
        LIMIT 1
    ), 0)
    INTO v_party;

    v_opening := v_hist + v_party;

    RETURN QUERY
    WITH opening AS (
        SELECT
            p_from_date::DATE AS d,
            'Opening Balance'::TEXT AS p,
            CASE WHEN v_opening >= 0 THEN v_opening ELSE 0::NUMERIC END AS dr,
            CASE WHEN v_opening < 0 THEN ABS(v_opening) ELSE 0::NUMERIC END AS cr,
            0 AS sort_key
    ),
    daily AS (
        SELECT
            j.jv_date::DATE AS d,
            'Summary'::TEXT AS p,
            SUM(CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0 END) AS dr,
            SUM(CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0 END) AS cr,
            1 AS sort_key
        FROM tab_jv j
        WHERE j.jv_comp_code = p_company_id
          AND (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
          AND j.jv_date::DATE BETWEEN p_from_date AND p_to_date
          AND (j.delete_flag IS NOT TRUE)
        GROUP BY j.jv_date::DATE
    ),
    combined AS (
        SELECT * FROM opening
        UNION ALL
        SELECT * FROM daily
    ),
    net_per_row AS (
        SELECT
            c.d,
            c.p,
            c.dr,
            c.cr,
            c.sort_key,
            CASE WHEN c.sort_key = 0 THEN v_opening ELSE (c.dr - c.cr) END AS net_amt
        FROM combined c
    ),
    flowed AS (
        SELECT
            n.d,
            n.p,
            n.dr,
            n.cr,
            n.sort_key,
            SUM(n.net_amt) OVER (ORDER BY n.d, n.sort_key ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS bal
        FROM net_per_row n
    ),
    closing_pick AS (
        SELECT f.bal AS closing_bal
        FROM flowed f
        ORDER BY f.d DESC NULLS LAST, f.sort_key DESC
        LIMIT 1
    ),
    day_totals AS (
        SELECT COALESCE(SUM(dr), 0)::NUMERIC AS tdr, COALESCE(SUM(cr), 0)::NUMERIC AS tcr
        FROM daily
    ),
    unioned AS (
        SELECT f.d, f.p, f.dr, f.cr, f.bal, 0 AS sec, f.d AS sd, f.sort_key AS sk
        FROM flowed f
        UNION ALL
        SELECT
            NULL::DATE,
            'Total'::TEXT,
            dt.tdr,
            dt.tcr,
            cp.closing_bal,
            1,
            NULL::DATE,
            0
        FROM day_totals dt
        CROSS JOIN closing_pick cp
    )
    SELECT u.d, u.p, u.dr, u.cr, u.bal
    FROM unioned u
    ORDER BY u.sec, u.sd NULLS FIRST, u.sk;
END;
$$;
