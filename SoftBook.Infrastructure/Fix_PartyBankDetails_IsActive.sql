-- Quick Fix: Remove is_active column reference from sp_party_bank_details_create
-- Run this against your SoftAccDb database

CREATE OR REPLACE FUNCTION sp_party_bank_details_create(
    p_company_id INT,
    p_party_id INT,
    p_account_holder_name VARCHAR,
    p_bank_name VARCHAR,
    p_account_number VARCHAR,
    p_ifsc VARCHAR,
    p_branch VARCHAR,
    p_micr VARCHAR,
    p_is_default BOOLEAN
)
RETURNS INT AS $$
DECLARE
    v_bank_id INT;
BEGIN
    INSERT INTO tab_party_bank_details (
        company_id, party_id, account_holder_name, bank_name, account_number, ifsc,
        branch, micr, is_default, created_on
    )
    VALUES (
        p_company_id, p_party_id, p_account_holder_name, p_bank_name, p_account_number, p_ifsc,
        p_branch, p_micr, COALESCE(p_is_default, FALSE), CURRENT_TIMESTAMP
    )
    RETURNING bank_id INTO v_bank_id;
    RETURN v_bank_id;
END;
$$ LANGUAGE plpgsql;
