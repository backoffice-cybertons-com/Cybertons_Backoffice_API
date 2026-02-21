-- FIX: Create missing sp_validate_user function
-- Run this in your 'companymaster' database query tool (pgAdmin)

CREATE OR REPLACE FUNCTION sp_validate_user(
    p_username VARCHAR,
    p_password_hash VARCHAR,
    p_company_id INT DEFAULT 1
)
RETURNS SETOF tab_user_master AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tab_user_master
    WHERE (username = p_username OR email = p_username)
      AND TRIM(password_hash) = TRIM(p_password_hash)
      AND (company_id = p_company_id OR p_company_id = 0);
END;
$$ LANGUAGE plpgsql;
