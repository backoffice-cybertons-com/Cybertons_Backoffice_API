-- =============================================
-- PARTY MASTER FUNCTIONS
-- =============================================

-- Create Party
DROP FUNCTION IF EXISTS sp_party_create(INT, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, BOOLEAN, VARCHAR, VARCHAR, INT, INT, INT, NUMERIC, VARCHAR, VARCHAR, BOOLEAN, VARCHAR, VARCHAR, VARCHAR);
CREATE OR REPLACE FUNCTION sp_party_create(
    p_company_id INT,
    p_party_type VARCHAR,
    p_salutation VARCHAR,
    p_first_name VARCHAR,
    p_last_name VARCHAR,
    p_company_name VARCHAR,
    p_display_name VARCHAR,
    p_email VARCHAR,
    p_phone_work VARCHAR,
    p_phone_mobile VARCHAR,
    p_preferred_language VARCHAR,
    p_gst_treatment VARCHAR,
    p_place_of_supply VARCHAR,
    p_pan VARCHAR,
    p_gst_no VARCHAR,
    p_msme_registered BOOLEAN,
    p_tax_preference VARCHAR,
    p_tds VARCHAR,
    p_currency_id INT,
    p_accounts_receivable_account INT,
    p_accounts_payable_account INT,
    p_opening_balance NUMERIC,
    p_opening_balance_type VARCHAR,
    p_payment_terms VARCHAR,
    p_enable_portal BOOLEAN,
    p_website_url VARCHAR,
    p_department VARCHAR,
    p_remarks VARCHAR
)
RETURNS INT AS $$
DECLARE
    v_party_id INT;
BEGIN
    INSERT INTO tab_party_master (
        company_id, party_type, salutation, first_name, last_name, company_name, display_name, email,
        phone_work, phone_mobile, preferred_language, gst_treatment, place_of_supply, pan, gst_no,
        msme_registered, tax_preference, tds, currency_id, accounts_receivable_account, accounts_payable_account,
        opening_balance, opening_balance_type, payment_terms, enable_portal, website_url, department, remarks,
        created_on
    )
    VALUES (
        p_company_id, p_party_type, p_salutation, p_first_name, p_last_name, p_company_name, p_display_name, p_email,
        p_phone_work, p_phone_mobile, p_preferred_language, p_gst_treatment, p_place_of_supply, p_pan, p_gst_no,
        p_msme_registered, p_tax_preference, p_tds, p_currency_id, p_accounts_receivable_account, p_accounts_payable_account,
        p_opening_balance, p_opening_balance_type, p_payment_terms, p_enable_portal, p_website_url, p_department, p_remarks,
        CURRENT_TIMESTAMP
    )
    RETURNING party_id INTO v_party_id;

    RETURN v_party_id;
END;
$$ LANGUAGE plpgsql;

-- Update Party
DROP FUNCTION IF EXISTS sp_party_update(INT, INT, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, BOOLEAN, VARCHAR, VARCHAR, INT, INT, INT, NUMERIC, VARCHAR, VARCHAR, BOOLEAN, VARCHAR, VARCHAR, VARCHAR);
CREATE OR REPLACE FUNCTION sp_party_update(
    p_party_id INT,
    p_company_id INT,
    p_party_type VARCHAR,
    p_salutation VARCHAR,
    p_first_name VARCHAR,
    p_last_name VARCHAR,
    p_company_name VARCHAR,
    p_display_name VARCHAR,
    p_email VARCHAR,
    p_phone_work VARCHAR,
    p_phone_mobile VARCHAR,
    p_preferred_language VARCHAR,
    p_gst_treatment VARCHAR,
    p_place_of_supply VARCHAR,
    p_pan VARCHAR,
    p_gst_no VARCHAR,
    p_msme_registered BOOLEAN,
    p_tax_preference VARCHAR,
    p_tds VARCHAR,
    p_currency_id INT,
    p_accounts_receivable_account INT,
    p_accounts_payable_account INT,
    p_opening_balance NUMERIC,
    p_opening_balance_type VARCHAR,
    p_payment_terms VARCHAR,
    p_enable_portal BOOLEAN,
    p_website_url VARCHAR,
    p_department VARCHAR,
    p_remarks VARCHAR
)
RETURNS VOID AS $$
BEGIN
    UPDATE tab_party_master
    SET
        party_type = p_party_type, salutation = p_salutation, first_name = p_first_name, last_name = p_last_name,
        company_name = p_company_name, display_name = p_display_name, email = p_email, phone_work = p_phone_work,
        phone_mobile = p_phone_mobile, preferred_language = p_preferred_language, gst_treatment = p_gst_treatment,
        place_of_supply = p_place_of_supply, pan = p_pan, gst_no = p_gst_no, msme_registered = p_msme_registered,
        tax_preference = p_tax_preference, tds = p_tds, currency_id = p_currency_id,
        accounts_receivable_account = p_accounts_receivable_account, accounts_payable_account = p_accounts_payable_account,
        opening_balance = p_opening_balance, opening_balance_type = p_opening_balance_type, payment_terms = p_payment_terms,
        enable_portal = p_enable_portal, website_url = p_website_url, department = p_department, remarks = p_remarks
    WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Delete Party
CREATE OR REPLACE FUNCTION sp_party_delete(
    p_party_id INT,
    p_company_id INT
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM tab_party_master WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get All Parties
CREATE OR REPLACE FUNCTION sp_party_get_all(
    p_company_id INT
)
RETURNS SETOF tab_party_master AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_party_master WHERE company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get Party By ID
CREATE OR REPLACE FUNCTION sp_party_get_by_id(
    p_party_id INT,
    p_company_id INT
)
RETURNS SETOF tab_party_master AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_party_master WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get Parties By Type
CREATE OR REPLACE FUNCTION sp_party_get_by_type(
    p_company_id INT,
    p_party_type VARCHAR
)
RETURNS SETOF tab_party_master AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_party_master 
    WHERE company_id = p_company_id 
      AND (party_type = p_party_type OR p_party_type IS NULL);
END;
$$ LANGUAGE plpgsql;


-- =============================================
-- PARTY ADDRESS FUNCTIONS
-- =============================================

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

CREATE OR REPLACE FUNCTION sp_party_address_update(
    p_address_id INT,
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
RETURNS VOID AS $$
BEGIN
    UPDATE tab_party_address
    SET
        party_id = p_party_id, address_type_id = p_address_type_id, attention = p_attention, country = p_country,
        address_line1 = p_address_line1, address_line2 = p_address_line2, city = p_city, state = p_state,
        pin_code = p_pin_code, phone = p_phone, fax = p_fax, is_default = COALESCE(p_is_default, FALSE)
    WHERE address_id = p_address_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_party_address_delete(
    p_address_id INT,
    p_company_id INT
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM tab_party_address WHERE address_id = p_address_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_party_address_get_by_party_id(
    p_party_id INT,
    p_company_id INT
)
RETURNS SETOF tab_party_address AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_party_address WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;


-- =============================================
-- PARTY BANK DETAILS FUNCTIONS
-- =============================================

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

CREATE OR REPLACE FUNCTION sp_party_bank_details_update(
    p_bank_id INT,
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
RETURNS VOID AS $$
BEGIN
    UPDATE tab_party_bank_details
    SET
        party_id = p_party_id, account_holder_name = p_account_holder_name, bank_name = p_bank_name,
        account_number = p_account_number, ifsc = p_ifsc,
        branch = p_branch, micr = p_micr, is_default = COALESCE(p_is_default, FALSE)
    WHERE bank_id = p_bank_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_party_bank_details_delete(
    p_bank_id INT,
    p_company_id INT
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM tab_party_bank_details WHERE bank_id = p_bank_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_party_bank_details_get_by_party_id(
    p_party_id INT,
    p_company_id INT
)
RETURNS SETOF tab_party_bank_details AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_party_bank_details WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;


-- =============================================
-- PARTY CONTACT PERSON FUNCTIONS
-- =============================================

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

CREATE OR REPLACE FUNCTION sp_party_contact_person_update(
    p_contact_person_id INT,
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
RETURNS VOID AS $$
BEGIN
    UPDATE tab_party_contact_persons
    SET
        party_id = p_party_id, salutation = p_salutation, first_name = p_first_name, last_name = p_last_name,
        email = p_email, work_phone = p_work_phone, mobile = p_mobile, is_primary = COALESCE(p_is_primary, FALSE)
    WHERE contact_person_id = p_contact_person_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_party_contact_person_delete(
    p_contact_person_id INT,
    p_company_id INT
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM tab_party_contact_persons WHERE contact_person_id = p_contact_person_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_party_contact_person_get_by_party_id(
    p_party_id INT,
    p_company_id INT
)
RETURNS SETOF tab_party_contact_persons AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_party_contact_persons WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;


-- =============================================
-- PARTY REPORTING TAGS FUNCTIONS
-- =============================================

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

CREATE OR REPLACE FUNCTION sp_party_reporting_tag_delete(
    p_tag_id INT,
    p_company_id INT
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM tab_party_reporting_tags WHERE tag_id = p_tag_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_party_reporting_tag_get_by_party_id(
    p_party_id INT,
    p_company_id INT
)
RETURNS SETOF tab_party_reporting_tags AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_party_reporting_tags WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;
