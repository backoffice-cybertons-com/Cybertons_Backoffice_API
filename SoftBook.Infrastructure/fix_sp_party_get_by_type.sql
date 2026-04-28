-- ============================================================
-- FIX: sp_party_get_by_type to support multiple comma-separated types
-- Run this in pgAdmin on your company database.
-- ============================================================

CREATE OR REPLACE FUNCTION sp_party_get_by_type(
    p_company_id INT,
    p_party_type VARCHAR
)
RETURNS SETOF tab_party_master AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_party_master 
    WHERE company_id = p_company_id 
      AND (p_party_type IS NULL OR party_type = ANY(string_to_array(p_party_type, ',')));
END;
$$ LANGUAGE plpgsql;

ALTER FUNCTION sp_party_get_by_type(INT, VARCHAR) OWNER TO postgres;
