-- MIGRATION SCRIPT: Party Master Extended Info Fields
-- Target: PostgreSQL
-- Date: 2026-06-22
-- Adds: SalespersonId, SalespersonMobile, Kam, RecoveryAgent, TransporterId, TransporterMobile to tab_party_master
-- Adds: Gstin, ContactPerson, Phone, Address to tab_salesperson_master

-- 1. DATABASE SCHEMA UPDATES
ALTER TABLE tab_party_master ADD COLUMN IF NOT EXISTS salesperson_id integer NULL;
ALTER TABLE tab_party_master ADD COLUMN IF NOT EXISTS salesperson_mobile varchar(50) NULL;
ALTER TABLE tab_party_master ADD COLUMN IF NOT EXISTS kam varchar(100) NULL;
ALTER TABLE tab_party_master ADD COLUMN IF NOT EXISTS recovery_agent varchar(100) NULL;
ALTER TABLE tab_party_master ADD COLUMN IF NOT EXISTS transporter_id integer NULL;
ALTER TABLE tab_party_master ADD COLUMN IF NOT EXISTS transporter_mobile varchar(50) NULL;

ALTER TABLE tab_salesperson_master ADD COLUMN IF NOT EXISTS gstin varchar(50) NULL;
ALTER TABLE tab_salesperson_master ADD COLUMN IF NOT EXISTS contact_person varchar(100) NULL;
ALTER TABLE tab_salesperson_master ADD COLUMN IF NOT EXISTS phone varchar(50) NULL;
ALTER TABLE tab_salesperson_master ADD COLUMN IF NOT EXISTS address varchar(255) NULL;

-- 2. UPDATE sp_party_create to include the new parameters
CREATE OR REPLACE FUNCTION sp_party_create(
    p_company_id INT,
    p_party_type VARCHAR,
    p_report_party_type VARCHAR DEFAULT NULL,
    p_salutation VARCHAR DEFAULT NULL,
    p_first_name VARCHAR DEFAULT NULL,
    p_last_name VARCHAR DEFAULT NULL,
    p_company_name VARCHAR DEFAULT NULL,
    p_display_name VARCHAR DEFAULT NULL,
    p_email VARCHAR DEFAULT NULL,
    p_phone_work VARCHAR DEFAULT NULL,
    p_phone_mobile VARCHAR DEFAULT NULL,
    p_preferred_language VARCHAR DEFAULT NULL,
    p_gst_treatment VARCHAR DEFAULT NULL,
    p_place_of_supply VARCHAR DEFAULT NULL,
    p_pan VARCHAR DEFAULT NULL,
    p_gst_no VARCHAR DEFAULT NULL,
    p_msme_registered BOOLEAN DEFAULT NULL,
    p_tax_preference VARCHAR DEFAULT NULL,
    p_tds VARCHAR DEFAULT NULL,
    p_currency_id INT DEFAULT NULL,
    p_accounts_receivable_account INT DEFAULT NULL,
    p_accounts_payable_account INT DEFAULT NULL,
    p_opening_balance NUMERIC DEFAULT NULL,
    p_opening_balance_type VARCHAR DEFAULT NULL,
    p_payment_terms VARCHAR DEFAULT NULL,
    p_enable_portal BOOLEAN DEFAULT NULL,
    p_website_url VARCHAR DEFAULT NULL,
    p_department VARCHAR DEFAULT NULL,
    p_remarks TEXT DEFAULT NULL,
    p_party_profile VARCHAR DEFAULT NULL,
    p_rate_diff NUMERIC DEFAULT 0,
    p_kam_mobile VARCHAR DEFAULT NULL,
    p_recovery_mobile VARCHAR DEFAULT NULL,
    p_sumul_incharge VARCHAR DEFAULT NULL,
    p_retailer_code VARCHAR DEFAULT NULL,
    p_sumul_depo_code VARCHAR DEFAULT NULL,
    p_pan_doc_path VARCHAR DEFAULT NULL,
    p_aadhaar_doc_path VARCHAR DEFAULT NULL,
    p_gst_cert_path VARCHAR DEFAULT NULL,
    p_user_id INT DEFAULT NULL,
    p_salesperson_id INT DEFAULT NULL,
    p_salesperson_mobile VARCHAR DEFAULT NULL,
    p_kam VARCHAR DEFAULT NULL,
    p_recovery_agent VARCHAR DEFAULT NULL,
    p_transporter_id INT DEFAULT NULL,
    p_transporter_mobile VARCHAR DEFAULT NULL
)
RETURNS INT AS $$
DECLARE
    v_party_id INT;
BEGIN
    INSERT INTO tab_party_master (
        company_id, party_type, report_party_type, salutation, first_name, last_name, company_name, display_name,
        email, phone_work, phone_mobile, preferred_language, gst_treatment, place_of_supply,
        pan, gst_no, msme_registered, tax_preference, tds, currency_id,
        accounts_receivable_account, accounts_payable_account, opening_balance, opening_balance_type,
        payment_terms, enable_portal, website_url, department, remarks, party_profile, rate_diff,
        kam_mobile, recovery_mobile, sumul_incharge, retailer_code, sumul_depo_code,
        pan_doc_path, aadhaar_doc_path, gst_cert_path,
        salesperson_id, salesperson_mobile, kam, recovery_agent, transporter_id, transporter_mobile,
        created_on
    )
    VALUES (
        p_company_id, p_party_type, COALESCE(p_report_party_type, p_party_type), p_salutation, p_first_name, p_last_name, p_company_name, p_display_name,
        p_email, p_phone_work, p_phone_mobile, p_preferred_language, p_gst_treatment, p_place_of_supply,
        p_pan, p_gst_no, p_msme_registered, p_tax_preference, p_tds, p_currency_id,
        p_accounts_receivable_account, p_accounts_payable_account, p_opening_balance, p_opening_balance_type,
        p_payment_terms, p_enable_portal, p_website_url, p_department, p_remarks, p_party_profile, p_rate_diff,
        p_kam_mobile, p_recovery_mobile, p_sumul_incharge, p_retailer_code, p_sumul_depo_code,
        p_pan_doc_path, p_aadhaar_doc_path, p_gst_cert_path,
        p_salesperson_id, p_salesperson_mobile, p_kam, p_recovery_agent, p_transporter_id, p_transporter_mobile,
        CURRENT_TIMESTAMP
    )
    RETURNING party_id INTO v_party_id;
    
    RETURN v_party_id;
END;
$$ LANGUAGE plpgsql;

-- 3. UPDATE sp_party_update to include the new parameters
CREATE OR REPLACE FUNCTION sp_party_update(
    p_party_id INT,
    p_company_id INT,
    p_party_type VARCHAR,
    p_report_party_type VARCHAR DEFAULT NULL,
    p_salutation VARCHAR DEFAULT NULL,
    p_first_name VARCHAR DEFAULT NULL,
    p_last_name VARCHAR DEFAULT NULL,
    p_company_name VARCHAR DEFAULT NULL,
    p_display_name VARCHAR DEFAULT NULL,
    p_email VARCHAR DEFAULT NULL,
    p_phone_work VARCHAR DEFAULT NULL,
    p_phone_mobile VARCHAR DEFAULT NULL,
    p_preferred_language VARCHAR DEFAULT NULL,
    p_gst_treatment VARCHAR DEFAULT NULL,
    p_place_of_supply VARCHAR DEFAULT NULL,
    p_pan VARCHAR DEFAULT NULL,
    p_gst_no VARCHAR DEFAULT NULL,
    p_msme_registered BOOLEAN DEFAULT NULL,
    p_tax_preference VARCHAR DEFAULT NULL,
    p_tds VARCHAR DEFAULT NULL,
    p_currency_id INT DEFAULT NULL,
    p_accounts_receivable_account INT DEFAULT NULL,
    p_accounts_payable_account INT DEFAULT NULL,
    p_opening_balance NUMERIC DEFAULT NULL,
    p_opening_balance_type VARCHAR DEFAULT NULL,
    p_payment_terms VARCHAR DEFAULT NULL,
    p_enable_portal BOOLEAN DEFAULT NULL,
    p_website_url VARCHAR DEFAULT NULL,
    p_department VARCHAR DEFAULT NULL,
    p_remarks TEXT DEFAULT NULL,
    p_party_profile VARCHAR DEFAULT NULL,
    p_rate_diff NUMERIC DEFAULT 0,
    p_kam_mobile VARCHAR DEFAULT NULL,
    p_recovery_mobile VARCHAR DEFAULT NULL,
    p_sumul_incharge VARCHAR DEFAULT NULL,
    p_retailer_code VARCHAR DEFAULT NULL,
    p_sumul_depo_code VARCHAR DEFAULT NULL,
    p_pan_doc_path VARCHAR DEFAULT NULL,
    p_aadhaar_doc_path VARCHAR DEFAULT NULL,
    p_gst_cert_path VARCHAR DEFAULT NULL,
    p_user_id INT DEFAULT NULL,
    p_salesperson_id INT DEFAULT NULL,
    p_salesperson_mobile VARCHAR DEFAULT NULL,
    p_kam VARCHAR DEFAULT NULL,
    p_recovery_agent VARCHAR DEFAULT NULL,
    p_transporter_id INT DEFAULT NULL,
    p_transporter_mobile VARCHAR DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    UPDATE tab_party_master
    SET
        party_type = p_party_type,
        report_party_type = COALESCE(p_report_party_type, p_party_type),
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
        kam_mobile = p_kam_mobile,
        recovery_mobile = p_recovery_mobile,
        sumul_incharge = p_sumul_incharge,
        retailer_code = p_retailer_code,
        sumul_depo_code = p_sumul_depo_code,
        pan_doc_path = p_pan_doc_path,
        aadhaar_doc_path = p_aadhaar_doc_path,
        gst_cert_path = p_gst_cert_path,
        salesperson_id = p_salesperson_id,
        salesperson_mobile = p_salesperson_mobile,
        kam = p_kam,
        recovery_agent = p_recovery_agent,
        transporter_id = p_transporter_id,
        transporter_mobile = p_transporter_mobile,
        modified_on = CURRENT_TIMESTAMP
    WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;
