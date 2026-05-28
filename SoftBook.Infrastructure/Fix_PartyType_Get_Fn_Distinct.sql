DROP FUNCTION IF EXISTS public.fn_get_all_party_types(INTEGER);

CREATE OR REPLACE FUNCTION public.fn_get_all_party_types(p_company_id INTEGER)
RETURNS TABLE (
    party_type_id INTEGER,
    company_id INTEGER,
    party_type_code VARCHAR,
    party_type_name VARCHAR,
    description TEXT,
    is_customer BOOLEAN,
    is_vendor BOOLEAN,
    is_active BOOLEAN,
    sort_order INTEGER,
    created_on TIMESTAMPTZ,
    account_nature VARCHAR,
    report_type VARCHAR,
    normal_balance VARCHAR,
    report_group VARCHAR,
    is_dynamic_nature BOOLEAN
)
LANGUAGE plpgsql
AS
$$
BEGIN
    RETURN QUERY
    SELECT 
        q.party_type_id,
        q.company_id,
        q.party_type_code,
        q.party_type_name,
        q.description,
        q.is_customer,
        q.is_vendor,
        q.is_active,
        q.sort_order,
        q.created_on,
        q.account_nature,
        q.report_type,
        q.normal_balance,
        q.report_group,
        q.is_dynamic_nature
    FROM (
        SELECT DISTINCT ON (UPPER(p.party_type_name))
            p.party_type_id,
            p.company_id,
            p.party_type_code,
            p.party_type_name,
            p.description,
            p.is_customer,
            p.is_vendor,
            p.is_active,
            p.sort_order,
            p.created_on,
            p.account_nature,
            p.report_type,
            p.normal_balance,
            p.report_group,
            p.is_dynamic_nature
        FROM public.tab_party_type_master p
        WHERE p.company_id IN (0, p_company_id) 
          AND p.is_active = true
        ORDER BY UPPER(p.party_type_name), p.company_id DESC
    ) q
    ORDER BY q.company_id, q.sort_order, q.party_type_name;
END;
$$;
