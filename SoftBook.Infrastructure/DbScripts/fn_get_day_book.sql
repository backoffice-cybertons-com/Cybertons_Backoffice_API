DROP FUNCTION IF EXISTS public.fn_get_day_book(INTEGER, DATE, DATE);

CREATE OR REPLACE FUNCTION public.fn_get_day_book(p_company_id INTEGER, p_from_date DATE, p_to_date DATE)
RETURNS TABLE(
    jv_date DATE,
    jv_code BIGINT,
    v_type TEXT,
    dr_account_name TEXT,
    cr_account_name TEXT,
    amount NUMERIC,
    description TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        j.jv_date::DATE,
        j.jv_code::BIGINT,
        j.v_type::TEXT,
        COALESCE(NULLIF(TRIM(dr.display_name), ''), NULLIF(TRIM(dr.company_name), ''), dr.party_id::TEXT) AS dr_account_name,
        COALESCE(NULLIF(TRIM(cr.display_name), ''), NULLIF(TRIM(cr.company_name), ''), cr.party_id::TEXT) AS cr_account_name,
        j.amount::NUMERIC,
        j.description::TEXT
    FROM public.tab_jv j
    LEFT JOIN public.tab_party_master dr ON j.dr_account_code = dr.party_id AND dr.company_id = p_company_id
    LEFT JOIN public.tab_party_master cr ON j.cr_account_code = cr.party_id AND cr.company_id = p_company_id
    WHERE j.jv_comp_code = p_company_id
      AND j.jv_date >= p_from_date
      AND j.jv_date < p_to_date + INTERVAL '1 day'
      AND COALESCE(j.delete_flag, FALSE) = FALSE
    ORDER BY j.jv_date, j.jv_code;
END;
$$ LANGUAGE plpgsql;
