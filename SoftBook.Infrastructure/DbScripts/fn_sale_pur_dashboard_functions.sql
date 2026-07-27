-- ============================================================
-- SoftBook: Dashboard & Party Rate Adjustment DB Functions
-- Run this script in pgAdmin on your SoftAccDb company database
-- Created: 2026-06-13
-- ============================================================


-- ============================================================
-- 0. fn_get_sales_invoices_for_add_price_check
--    Returns sales invoices within a date range with the actual
--    add price applied per invoice line (for mismatch checking).
-- ============================================================
CREATE OR REPLACE FUNCTION public.fn_get_sales_invoices_for_add_price_check(
    p_company_id        INT,
    p_from_date         DATE,
    p_to_date           DATE,
    p_party_type        VARCHAR DEFAULT NULL,
    p_fiscal_year_id    INT     DEFAULT NULL
)
RETURNS TABLE (
    sp_unique_code      INT,
    inv_no              VARCHAR,
    inv_date            DATE,
    due_date            DATE,
    party_id            INT,
    display_name        VARCHAR,
    party_type          VARCHAR,
    actual_add_price    NUMERIC
) AS $$
    SELECT
        sp.sp_unique_code,
        COALESCE(sp.inv_no, '')::VARCHAR                                AS inv_no,
        sp.inv_date::DATE,
        sp.due_date::DATE,
        sp.party_id,
        COALESCE(pm.display_name, '')::VARCHAR                          AS display_name,
        COALESCE(pm.party_type,   '')::VARCHAR                          AS party_type,
        COALESCE(line_adj.actual_add_price, sp.rate_diff, 0)::NUMERIC   AS actual_add_price
    FROM public.tab_sale_pur sp
    INNER JOIN public.tab_party_master pm
        ON pm.party_id    = sp.party_id
       AND pm.company_id  = sp.company_id
    LEFT JOIN LATERAL (
        SELECT MAX(COALESCE(t.rate_diff, 0)) AS actual_add_price
        FROM public.tab_sal_pur_tran t
        WHERE t.sp_unique_code = sp.sp_unique_code
          AND COALESCE(t.delete_flag, FALSE) = FALSE
    ) line_adj ON TRUE
    WHERE sp.company_id = p_company_id
      AND LOWER(TRIM(sp.inv_type)) = 'sales'
      AND sp.inv_date >= p_from_date
      AND sp.inv_date < (p_to_date + INTERVAL '1 day')
      AND COALESCE(sp.delete_flag, FALSE) = FALSE
      AND (p_party_type IS NULL OR pm.party_type = ANY(string_to_array(p_party_type, ',')))
      AND (p_fiscal_year_id IS NULL OR sp.fiscal_year_id = p_fiscal_year_id)
    ORDER BY sp.inv_date DESC, sp.sp_unique_code DESC;
$$ LANGUAGE sql STABLE;

ALTER FUNCTION public.fn_get_sales_invoices_for_add_price_check(INT, DATE, DATE, VARCHAR, INT) OWNER TO postgres;


-- ============================================================
-- 1. fn_get_party_outstanding_before_date
--    Returns net outstanding balance for a party strictly
--    BEFORE the given date (JV debit - credit + opening balance)
-- ============================================================
CREATE OR REPLACE FUNCTION public.fn_get_party_outstanding_before_date(
    p_company_id    INT,
    p_party_id      INT,
    p_before_date   DATE
)
RETURNS NUMERIC AS $$
DECLARE
    v_ar_account    INT;
    v_ap_account    INT;
    v_opening_bal   NUMERIC;
    v_jv_balance    NUMERIC;
BEGIN
    SELECT
        COALESCE(accounts_receivable_account, p_party_id),
        COALESCE(accounts_payable_account,   p_party_id),
        CASE
            WHEN UPPER(opening_balance_type) = 'DR' THEN  COALESCE(opening_balance, 0)::NUMERIC
            WHEN UPPER(opening_balance_type) = 'CR' THEN -COALESCE(opening_balance, 0)::NUMERIC
            ELSE 0
        END
    INTO v_ar_account, v_ap_account, v_opening_bal
    FROM public.tab_party_master
    WHERE party_id = p_party_id
    LIMIT 1;

    SELECT COALESCE(
        SUM(CASE WHEN j.dr_account_code IN (v_ar_account, v_ap_account) THEN j.amount ELSE 0 END) -
        SUM(CASE WHEN j.cr_account_code IN (v_ar_account, v_ap_account) THEN j.amount ELSE 0 END),
    0)
    INTO v_jv_balance
    FROM public.tab_jv j
    WHERE j.jv_comp_code = p_company_id
      AND (j.dr_account_code IN (v_ar_account, v_ap_account)
           OR j.cr_account_code IN (v_ar_account, v_ap_account))
      AND j.jv_date < p_before_date
      AND (j.delete_flag = FALSE OR j.delete_flag IS NULL);

    RETURN COALESCE(v_jv_balance, 0) + COALESCE(v_opening_bal, 0);
END;
$$ LANGUAGE plpgsql STABLE;

ALTER FUNCTION public.fn_get_party_outstanding_before_date(INT, INT, DATE) OWNER TO postgres;


-- ============================================================
-- 2. fn_get_party_todays_receipt
--    Returns total receipt (credit JV) for a party on a given date
-- ============================================================
CREATE OR REPLACE FUNCTION public.fn_get_party_todays_receipt(
    p_company_id    INT,
    p_party_id      INT,
    p_date          DATE
)
RETURNS NUMERIC AS $$
DECLARE
    v_ar_account    INT;
    v_ap_account    INT;
    v_total         NUMERIC;
BEGIN
    SELECT
        COALESCE(accounts_receivable_account, p_party_id),
        COALESCE(accounts_payable_account,   p_party_id)
    INTO v_ar_account, v_ap_account
    FROM public.tab_party_master
    WHERE party_id = p_party_id
    LIMIT 1;

    SELECT COALESCE(
        SUM(CASE WHEN j.cr_account_code IN (v_ar_account, v_ap_account) THEN j.amount ELSE 0 END),
    0)
    INTO v_total
    FROM public.tab_jv j
    WHERE j.jv_comp_code = p_company_id
      AND j.jv_date >= p_date AND j.jv_date < (p_date + INTERVAL '1 day')
      AND j.cr_account_code IN (v_ar_account, v_ap_account)
      AND (j.delete_flag = FALSE OR j.delete_flag IS NULL);

    RETURN COALESCE(v_total, 0);
END;
$$ LANGUAGE plpgsql STABLE;

ALTER FUNCTION public.fn_get_party_todays_receipt(INT, INT, DATE) OWNER TO postgres;


-- ============================================================
-- 3. fn_get_party_todays_invoice_amount
--    Returns total sales invoice grand total for a party on a given date
-- ============================================================
CREATE OR REPLACE FUNCTION public.fn_get_party_todays_invoice_amount(
    p_company_id    INT,
    p_party_id      INT,
    p_date          DATE
)
RETURNS NUMERIC AS $$
DECLARE
    v_total NUMERIC;
BEGIN
    SELECT COALESCE(SUM(grand_total), 0)
    INTO v_total
    FROM public.tab_sale_pur
    WHERE company_id = p_company_id
      AND party_id   = p_party_id
      AND LOWER(TRIM(inv_type)) = 'sales'
      AND inv_date >= p_date AND inv_date < (p_date + INTERVAL '1 day')
      AND (delete_flag = FALSE OR delete_flag IS NULL);

    RETURN COALESCE(v_total, 0);
END;
$$ LANGUAGE plpgsql STABLE;

ALTER FUNCTION public.fn_get_party_todays_invoice_amount(INT, INT, DATE) OWNER TO postgres;


-- ============================================================
-- 4. fn_get_party_last7days_sales
--    Returns total sales grand_total for a party for the 7 days
--    window ending the day BEFORE as_of_date (excludes today)
--    Optional: filter by fiscal_year_id (pass NULL to skip)
-- ============================================================
CREATE OR REPLACE FUNCTION public.fn_get_party_last7days_sales(
    p_company_id        INT,
    p_party_id          INT,
    p_as_of_date        DATE,
    p_fiscal_year_id    INT DEFAULT NULL
)
RETURNS NUMERIC AS $$
DECLARE
    v_total NUMERIC;
BEGIN
    SELECT COALESCE(SUM(grand_total), 0)
    INTO v_total
    FROM public.tab_sale_pur
    WHERE company_id = p_company_id
      AND party_id   = p_party_id
      AND LOWER(TRIM(inv_type)) = 'sales'
      AND inv_date >= (p_as_of_date - INTERVAL '7 day')
      AND inv_date < p_as_of_date
      AND COALESCE(delete_flag, FALSE) = FALSE
      AND (p_fiscal_year_id IS NULL OR fiscal_year_id = p_fiscal_year_id);

    RETURN COALESCE(v_total, 0);
END;
$$ LANGUAGE plpgsql STABLE;

ALTER FUNCTION public.fn_get_party_last7days_sales(INT, INT, DATE, INT) OWNER TO postgres;


-- ============================================================
-- 5. fn_get_party_rate_adjustment_bulk
--    Bulk query: returns all parties of given types with their
--    7-day sales total and net outstanding in ONE query.
--    This replaces the N+1 loop that caused 50-second load times.
-- ============================================================
CREATE OR REPLACE FUNCTION public.fn_get_party_rate_adjustment_bulk(
    p_company_id        INT,
    p_party_types       VARCHAR,        -- comma-separated e.g. 'Customer,Trader'
    p_as_of_date        DATE,
    p_fiscal_year_id    INT DEFAULT NULL
)
RETURNS TABLE (
    party_id                INT,
    display_name            VARCHAR,
    party_type              VARCHAR,
    payment_terms           VARCHAR,
    stored_rate_diff        NUMERIC,
    last_7_days_sales_total NUMERIC,
    total_outstanding       NUMERIC
) AS $$
    SELECT
        p.party_id,
        p.display_name,
        p.party_type,
        p.payment_terms,
        COALESCE(p.rate_diff, 0)::NUMERIC                   AS stored_rate_diff,
        COALESCE(sales.total, 0)::NUMERIC                   AS last_7_days_sales_total,
        (
            COALESCE(jv.net_balance, 0) +
            CASE
                WHEN UPPER(p.opening_balance_type) = 'DR' THEN  COALESCE(p.opening_balance, 0)::NUMERIC
                WHEN UPPER(p.opening_balance_type) = 'CR' THEN -COALESCE(p.opening_balance, 0)::NUMERIC
                ELSE 0
            END
        )::NUMERIC                                          AS total_outstanding
    FROM public.tab_party_master p

    LEFT JOIN LATERAL (
        SELECT SUM(s.grand_total) AS total
        FROM public.tab_sale_pur s
        WHERE s.company_id = p.company_id
          AND s.party_id   = p.party_id
          AND LOWER(TRIM(s.inv_type)) = 'sales'
          AND s.inv_date >= (p_as_of_date - INTERVAL '7 day')
          AND s.inv_date < p_as_of_date
          AND COALESCE(s.delete_flag, FALSE) = FALSE
          AND (p_fiscal_year_id IS NULL OR s.fiscal_year_id = p_fiscal_year_id)
    ) sales ON TRUE

    LEFT JOIN LATERAL (
        SELECT
            (SELECT COALESCE(SUM(j.amount), 0)
             FROM public.tab_jv j
             WHERE j.jv_comp_code = p.company_id
               AND j.dr_account_code IN (COALESCE(p.accounts_receivable_account, p.party_id), COALESCE(p.accounts_payable_account, p.party_id))
               AND j.jv_date < p_as_of_date
               AND COALESCE(j.delete_flag, FALSE) = FALSE)
            -
            (SELECT COALESCE(SUM(j.amount), 0)
             FROM public.tab_jv j
             WHERE j.jv_comp_code = p.company_id
               AND j.cr_account_code IN (COALESCE(p.accounts_receivable_account, p.party_id), COALESCE(p.accounts_payable_account, p.party_id))
               AND j.jv_date < p_as_of_date
               AND COALESCE(j.delete_flag, FALSE) = FALSE) AS net_balance
    ) jv ON TRUE

    WHERE (p_company_id = 0 OR p.company_id = p_company_id)
      AND p.party_type = ANY(string_to_array(p_party_types, ','));
$$ LANGUAGE sql STABLE;

ALTER FUNCTION public.fn_get_party_rate_adjustment_bulk(INT, VARCHAR, DATE, INT) OWNER TO postgres;


-- ============================================================
-- 6. fn_get_add_price_lookups
--    Returns all add price tier rows for a company.
--    Used for in-memory matching in the backend.
-- ============================================================
CREATE OR REPLACE FUNCTION public.fn_get_add_price_lookups(
    p_company_id INT
)
RETURNS TABLE (
    add_price   NUMERIC,
    from_due    NUMERIC,
    to_due      NUMERIC
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        COALESCE(ap.add_price, 0)::NUMERIC,
        COALESCE(ap.from_due,  0)::NUMERIC,
        COALESCE(ap.to_due,    0)::NUMERIC
    FROM public.tab_add_price ap
    WHERE (ap.company_id IS NULL OR ap.company_id = p_company_id)
    ORDER BY ap.from_due;
END;
$$ LANGUAGE plpgsql STABLE;

ALTER FUNCTION public.fn_get_add_price_lookups(INT) OWNER TO postgres;


-- ============================================================
-- 7. fn_get_add_price_for_days
--    Returns the matching add price tier for a given
--    avg outstanding days value.
-- ============================================================
CREATE OR REPLACE FUNCTION public.fn_get_add_price_for_days(
    p_company_id        INT,
    p_avg_os_days       NUMERIC
)
RETURNS TABLE (
    add_price   NUMERIC,
    from_due    NUMERIC,
    to_due      NUMERIC
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        COALESCE(ap.add_price, 0)::NUMERIC,
        COALESCE(ap.from_due,  0)::NUMERIC,
        COALESCE(ap.to_due,    0)::NUMERIC
    FROM public.tab_add_price ap
    WHERE p_avg_os_days >= ap.from_due
      AND p_avg_os_days <= ap.to_due
      AND (ap.company_id IS NULL OR ap.company_id = p_company_id)
    ORDER BY ap.rowid
    LIMIT 1;
END;
$$ LANGUAGE plpgsql STABLE;

ALTER FUNCTION public.fn_get_add_price_for_days(INT, NUMERIC) OWNER TO postgres;


-- ============================================================
-- RECOMMENDED INDEXES (run once — safe to re-run)
-- These dramatically speed up the LATERAL joins above
-- ============================================================

-- Index for 7-day sales window lookup
CREATE INDEX IF NOT EXISTS idx_sale_pur_party_date
    ON public.tab_sale_pur (company_id, party_id, inv_date, delete_flag);

-- Index for invoice date ranges (Massive speedup for yearly reports)
CREATE INDEX IF NOT EXISTS idx_sale_pur_inv_date
    ON public.tab_sale_pur (company_id, inv_type, inv_date, delete_flag);

-- Index for JV outstanding balance lookup (Dr)
CREATE INDEX IF NOT EXISTS idx_jv_comp_date_dr
    ON public.tab_jv (jv_comp_code, dr_account_code, jv_date, delete_flag);

-- Index for JV outstanding balance lookup (Cr)
CREATE INDEX IF NOT EXISTS idx_jv_comp_date_cr
    ON public.tab_jv (jv_comp_code, cr_account_code, jv_date, delete_flag);

-- Index for invoice lines (Massive speedup for add_price mismatch check)
CREATE INDEX IF NOT EXISTS idx_sal_pur_tran_sp_unique_code
    ON public.tab_sal_pur_tran (sp_unique_code, delete_flag);

-- ============================================================
-- Done! Run this script in pgAdmin, then restart the backend.
-- ============================================================
