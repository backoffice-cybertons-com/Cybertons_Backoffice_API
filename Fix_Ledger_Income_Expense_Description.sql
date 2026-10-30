-- =============================================================================
-- FIX: fn_ledger_report_v2 – Show offsetting account name for Income/Expense
-- Problem: Income/Expense (direct journal) entries showed blank in Particulars
--          because the old 4-param fn just used COALESCE(j.description, '').
-- Solution: Add LATERAL joins (like Update_Ledger_Particulars.sql) to resolve
--           the DR / CR account name and always show:
--           "<Offsetting Account Name>" + newline + "<description if any>"
-- Apply to: Live PostgreSQL database (per-tenant schema)
-- Date: 2026-06-02
-- =============================================================================

-- Drop old signatures to avoid overload conflicts
DROP FUNCTION IF EXISTS fn_ledger_report_v2(INT, TIMESTAMP, TIMESTAMP);
DROP FUNCTION IF EXISTS fn_ledger_report_v2(INT, DATE, DATE);
DROP FUNCTION IF EXISTS fn_ledger_report_v2(INT, INT, DATE, DATE);

CREATE OR REPLACE FUNCTION fn_ledger_report_v2(
    p_company_id   INT,
    p_account_code INT,
    p_from_date    DATE,
    p_to_date      DATE
)
RETURNS TABLE
(
    res_jv_date     DATE,
    res_v_type      TEXT,
    res_v_code      TEXT,
    res_description TEXT,
    res_debit       NUMERIC,
    res_credit      NUMERIC,
    res_balance     NUMERIC,
    res_id          BIGINT
)
AS
$$
DECLARE
    v_opening_balance NUMERIC := 0.0;
    v_party_opening   NUMERIC := 0.0;
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
                WHEN UPPER(TRIM(opening_balance_type)) IN ('DR', 'DEBIT', 'D', 'RECEIVABLE', 'REC', 'DEBT') THEN  ABS(COALESCE(opening_balance, 0))::NUMERIC
                WHEN UPPER(TRIM(opening_balance_type)) IN ('CR', 'CREDIT', 'C', 'PAYABLE', 'PAY', 'CRED')  THEN -ABS(COALESCE(opening_balance, 0))::NUMERIC
                ELSE 0
            END
        FROM tab_party_master
        WHERE party_id = p_account_code
           OR accounts_receivable_account = p_account_code
           OR accounts_payable_account    = p_account_code
        LIMIT 1
    ), 0.0) INTO v_party_opening;

    v_opening_balance := v_opening_balance + v_party_opening;

    -----------------------------------------
    -- 3. Return Unified Data Set
    --    For Income/Expense entries the "Particulars" now shows:
    --      <Offsetting account name>
    --      <user description, if entered>
    -----------------------------------------
    RETURN QUERY
    WITH ledger_rows AS (
        -- [A] Opening Balance Row
        SELECT
            p_from_date::DATE                                                   AS d,
            'Opening Balance'::TEXT                                             AS t,
            ''::TEXT                                                            AS c,
            'Opening Balance b/f'::TEXT                                         AS ds,
            CASE WHEN v_opening_balance >= 0 THEN v_opening_balance ELSE 0.0 END AS dr,
            CASE WHEN v_opening_balance <  0 THEN ABS(v_opening_balance) ELSE 0.0 END AS cr,
            v_opening_balance                                                   AS amt,
            0                                                                   AS s,
            0::BIGINT                                                           AS id

        UNION ALL

        -- [B] Transaction Rows – resolve offsetting account name via LATERAL
        SELECT
            j.jv_date::DATE,

            -- v_type display
            CASE
                WHEN j.v_type IS NOT NULL AND UPPER(j.v_type) <> 'JV' THEN j.v_type
                WHEN j.description LIKE '% Output - Inv %'             THEN 'Sales'
                WHEN j.description LIKE '% Input - Bill %'             THEN 'Purchase'
                ELSE COALESCE(j.v_type, 'JV')
            END::TEXT,

            -- voucher number / edit key:
            -- Payment/Receipt → jv_code; Income/Expense → g_code (JV-...); Sale/Purchase → v_code
            CASE
                WHEN UPPER(j.v_type) IN ('PAYMENT', 'RECEIPT') THEN j.jv_code::TEXT
                WHEN UPPER(COALESCE(j.v_type, '')) IN ('INCOME', 'EXPENSE') THEN COALESCE(j.g_code, '')::TEXT
                ELSE COALESCE(j.v_code, 0)::TEXT
            END,

            -- *** PARTICULARS: offsetting account name + optional description ***
            (CASE
                WHEN j.dr_account_code = p_account_code THEN
                    -- This account was DEBITED  → show CR account (who credited us)
                    COALESCE(cr_acc.name, 'Unknown Account')
                    || CASE WHEN COALESCE(j.description, '') <> '' THEN CHR(10) || j.description ELSE '' END
                WHEN j.cr_account_code = p_account_code THEN
                    -- This account was CREDITED → show DR account (who debited)
                    COALESCE(dr_acc.name, 'Unknown Account')
                    || CASE WHEN COALESCE(j.description, '') <> '' THEN CHR(10) || j.description ELSE '' END
                ELSE
                    COALESCE(j.description, '')
            END)::TEXT,

            CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END,
            CASE WHEN j.cr_account_code = p_account_code THEN j.amount::NUMERIC ELSE 0.0 END,
            CASE WHEN j.dr_account_code = p_account_code THEN j.amount::NUMERIC ELSE -j.amount::NUMERIC END,
            1,
            j.jv_code

        FROM tab_jv j

        -- Resolve DR account name (party first, then account master)
        LEFT JOIN LATERAL (
            SELECT name FROM (
                SELECT display_name  AS name, 1 AS priority FROM tab_party_master   WHERE party_id  = j.dr_account_code
                UNION ALL
                SELECT account_name  AS name, 2 AS priority FROM tab_account_master WHERE account_id = j.dr_account_code
            ) n ORDER BY priority LIMIT 1
        ) dr_acc ON TRUE

        -- Resolve CR account name (party first, then account master)
        LEFT JOIN LATERAL (
            SELECT name FROM (
                SELECT display_name  AS name, 1 AS priority FROM tab_party_master   WHERE party_id  = j.cr_account_code
                UNION ALL
                SELECT account_name  AS name, 2 AS priority FROM tab_account_master WHERE account_id = j.cr_account_code
            ) n ORDER BY priority LIMIT 1
        ) cr_acc ON TRUE

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
