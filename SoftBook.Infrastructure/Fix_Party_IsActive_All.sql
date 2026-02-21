-- Comprehensive Fix: Remove is_active column references from all party child table create functions
-- Run this against your SoftAccDb database

-- Fix sp_party_address_create
CREATE OR REPLACE FUNCTION sp_party_address_create(
    p_company_id INT,
    p_party_id INT,
    p_address_type_id INT,
    p_attention VARCHAR,
    p_country VARCHAR,
    p_address_line1 VARCHAR,
    p_address_line2 VARCHAR,
    p_city VARCHAR,
    p_state VARCHAR,
    p_pin_code VARCHAR,
    p_phone VARCHAR,
    p_fax VARCHAR,
    p_is_default BOOLEAN
)
RETURNS INT AS $$
DECLARE
    v_address_id INT;
BEGIN
    INSERT INTO tab_party_address (
        company_id, party_id, address_type_id, attention, country, address_line1, address_line2,
        city, state, pin_code, phone, fax, is_default, created_on
    )
    VALUES (
        p_company_id, p_party_id, p_address_type_id, p_attention, p_country, p_address_line1, p_address_line2,
        p_city, p_state, p_pin_code, p_phone, p_fax, COALESCE(p_is_default, FALSE), CURRENT_TIMESTAMP
    )
    RETURNING address_id INTO v_address_id;
    RETURN v_address_id;
END;
$$ LANGUAGE plpgsql;

-- Fix sp_party_bank_details_create
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

-- Fix sp_party_contact_person_create
CREATE OR REPLACE FUNCTION sp_party_contact_person_create(
    p_company_id INT,
    p_party_id INT,
    p_salutation VARCHAR,
    p_first_name VARCHAR,
    p_last_name VARCHAR,
    p_email VARCHAR,
    p_work_phone VARCHAR,
    p_mobile VARCHAR,
    p_is_primary BOOLEAN
)
RETURNS INT AS $$
DECLARE
    v_contact_person_id INT;
BEGIN
    INSERT INTO tab_party_contact_persons (
        company_id, party_id, salutation, first_name, last_name, email, work_phone, mobile,
        is_primary, created_on
    )
    VALUES (
        p_company_id, p_party_id, p_salutation, p_first_name, p_last_name, p_email, p_work_phone, p_mobile,
        COALESCE(p_is_primary, FALSE), CURRENT_TIMESTAMP
    )
    RETURNING contact_person_id INTO v_contact_person_id;
    RETURN v_contact_person_id;
END;
$$ LANGUAGE plpgsql;

-- Fix sp_party_reporting_tag_create
CREATE OR REPLACE FUNCTION sp_party_reporting_tag_create(
    p_company_id INT,
    p_party_id INT,
    p_tag_name VARCHAR
)
RETURNS INT AS $$
DECLARE
    v_tag_id INT;
BEGIN
    INSERT INTO tab_party_reporting_tags (
        company_id, party_id, tag_name, created_on
    )
    VALUES (
        p_company_id, p_party_id, p_tag_name, CURRENT_TIMESTAMP
    )
    RETURNING tag_id INTO v_tag_id;
    RETURN v_tag_id;
END;
$$ LANGUAGE plpgsql;
