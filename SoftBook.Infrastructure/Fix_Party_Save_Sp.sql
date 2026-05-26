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
    p_remarks VARCHAR,
    p_party_profile VARCHAR DEFAULT NULL,
    p_rate_diff NUMERIC DEFAULT 0,
    p_report_party_type VARCHAR DEFAULT NULL,
    p_user_id INT DEFAULT NULL
)
RETURNS INT AS $$
DECLARE
    v_party_id INT;
BEGIN
    INSERT INTO tab_party_master (
        company_id, party_type, salutation, first_name, last_name, company_name, display_name,
        email, phone_work, phone_mobile, preferred_language, gst_treatment, place_of_supply,
        pan, gst_no, msme_registered, tax_preference, tds, currency_id,
        accounts_receivable_account, accounts_payable_account, opening_balance,
        opening_balance_type, payment_terms, enable_portal, website_url,
        department, remarks, party_profile, rate_diff, report_party_type, created_on, modified_on
    ) VALUES (
        p_company_id, p_party_type, p_salutation, p_first_name, p_last_name, p_company_name, p_display_name,
        p_email, p_phone_work, p_phone_mobile, p_preferred_language, p_gst_treatment, p_place_of_supply,
        p_pan, p_gst_no, p_msme_registered, p_tax_preference, p_tds, p_currency_id,
        p_accounts_receivable_account, p_accounts_payable_account, p_opening_balance,
        p_opening_balance_type, p_payment_terms, p_enable_portal, p_website_url,
        p_department, p_remarks, p_party_profile, p_rate_diff, COALESCE(p_report_party_type, p_party_type), CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
    ) RETURNING party_id INTO v_party_id;

    RETURN v_party_id;
END;
$$ LANGUAGE plpgsql;


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
    p_remarks VARCHAR,
    p_party_profile VARCHAR DEFAULT NULL,
    p_rate_diff NUMERIC DEFAULT 0,
    p_report_party_type VARCHAR DEFAULT NULL,
    p_user_id INT DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    UPDATE tab_party_master
    SET
        party_type = p_party_type,
        salutation = p_salutation,
        first_name = p_first_name,
        last_name = p_last_name,
        company_name = p_company_name,
        display_name = p_display_name,
        email = p_email,
        phone_work = p_phone_work,
        phone_mobile = p_phone_mobile,
        preferred_language = p_preferred_language,
        gst_treatment = p_gst_treatment,
        place_of_supply = p_place_of_supply,
        pan = p_pan,
        gst_no = p_gst_no,
        msme_registered = p_msme_registered,
        tax_preference = p_tax_preference,
        tds = p_tds,
        currency_id = p_currency_id,
        accounts_receivable_account = p_accounts_receivable_account,
        accounts_payable_account = p_accounts_payable_account,
        opening_balance = p_opening_balance,
        opening_balance_type = p_opening_balance_type,
        payment_terms = p_payment_terms,
        enable_portal = p_enable_portal,
        website_url = p_website_url,
        department = p_department,
        remarks = p_remarks,
        party_profile = p_party_profile,
        rate_diff = p_rate_diff,
        report_party_type = COALESCE(p_report_party_type, p_party_type),
        modified_on = CURRENT_TIMESTAMP
    WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;
