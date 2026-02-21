-- PostgreSQL Function to Get Company By ID
-- Run this in 'companymaster' database

CREATE OR REPLACE FUNCTION sp_getcompanybyid(p_company_id INT)
RETURNS TABLE (
    company_id INT,
    company_name VARCHAR(100),
    connection_string VARCHAR(500),
    is_enabled BOOLEAN,
    priority INT,
    created_at TIMESTAMPTZ
)
LANGUAGE sql
AS $$
    SELECT
        t.company_id,
        t.company_name,
        t.connection_string,
        t.is_enabled,
        t.priority,
        t.created_at
    FROM tab_company_master t
    WHERE t.company_id = p_company_id;
$$;
