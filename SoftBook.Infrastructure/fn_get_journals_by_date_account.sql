-- Existing Income/Expense JV rows for selected date + account
-- Paste/run this in DB, then API uses: SELECT * FROM public.fn_get_journals_by_date_account(...)

DROP FUNCTION IF EXISTS public.fn_get_journals_by_date_account(INTEGER, DATE, BIGINT);

CREATE OR REPLACE FUNCTION public.fn_get_journals_by_date_account(
    p_company_id   INTEGER,
    p_jv_date      DATE,
    p_account_code BIGINT
)
RETURNS TABLE (
    jv_code          BIGINT,
    jv_date          TIMESTAMPTZ,
    v_type           TEXT,
    amount           NUMERIC,
    g_code           TEXT,
    description      TEXT,
    dr_account_code  BIGINT,
    cr_account_code  BIGINT,
    dr_account_name  TEXT,
    cr_account_name  TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        j.jv_code::BIGINT,
        j.jv_date,
        j.v_type::TEXT,
        j.amount::NUMERIC,
        j.g_code::TEXT,
        j.description::TEXT,
        j.dr_account_code::BIGINT,
        j.cr_account_code::BIGINT,
        COALESCE(dr_am.account_name, dr_pm.display_name, 'Unknown')::TEXT AS dr_account_name,
        COALESCE(cr_am.account_name, cr_pm.display_name, 'Unknown')::TEXT AS cr_account_name
    FROM public.tab_jv j
    LEFT JOIN public.tab_account_master dr_am ON dr_am.account_id = j.dr_account_code
    LEFT JOIN public.tab_party_master   dr_pm ON dr_pm.party_id   = j.dr_account_code
    LEFT JOIN public.tab_account_master cr_am ON cr_am.account_id = j.cr_account_code
    LEFT JOIN public.tab_party_master   cr_pm ON cr_pm.party_id   = j.cr_account_code
    WHERE j.jv_comp_code = p_company_id
      AND j.jv_date::date = p_jv_date
      AND (j.dr_account_code = p_account_code OR j.cr_account_code = p_account_code)
      AND COALESCE(j.delete_flag, FALSE) = FALSE
    ORDER BY j.jv_code DESC;
END;
$$ LANGUAGE plpgsql;
