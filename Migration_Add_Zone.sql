-- ============================================================
-- Migration: Add Zone Master and Zone to Party Master
-- Date: 2026-07-24
-- ============================================================

-- STEP 1: Create tab_zone_master
-- ============================================================
CREATE TABLE IF NOT EXISTS public.tab_zone_master (
    zone_id     SERIAL PRIMARY KEY,
    company_id  INTEGER NOT NULL,
    zone_name   VARCHAR(255) NOT NULL,
    is_active   BOOLEAN NOT NULL DEFAULT TRUE,
    created_on  TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Unique constraint: zone name must be unique per company
ALTER TABLE public.tab_zone_master
    DROP CONSTRAINT IF EXISTS uq_zone_name;
ALTER TABLE public.tab_zone_master
    ADD CONSTRAINT uq_zone_name UNIQUE (company_id, zone_name);

-- ============================================================
-- STEP 2: Add zone_id column to tab_party_master (if not exists)
-- ============================================================
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name = 'tab_party_master' AND column_name = 'zone_id'
    ) THEN
        ALTER TABLE public.tab_party_master ADD COLUMN zone_id INTEGER NULL;
    END IF;
END
$$;

-- ============================================================
-- STEP 3: Drop old versions of sp_party_create and sp_party_update
-- ============================================================
DROP FUNCTION IF EXISTS public.sp_party_create(integer, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, boolean, character varying, character varying, integer, integer, integer, numeric, character varying, character varying, boolean, character varying, character varying, character varying, character varying, numeric, character varying, integer);

DROP FUNCTION IF EXISTS public.sp_party_update(integer, integer, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, boolean, character varying, character varying, integer, integer, integer, numeric, character varying, character varying, boolean, character varying, character varying, character varying, character varying, numeric, character varying, integer);

DROP FUNCTION IF EXISTS public.sp_party_create(integer, text, text, text, text, text, text, text, text, text, text, text, text, text, text, boolean, text, text, integer, integer, integer, numeric, text, text, boolean, text, text, text, text, numeric, text, text, text, text, text, text, text, text, integer, integer, text, text, text, integer, text);

DROP FUNCTION IF EXISTS public.sp_party_update(integer, integer, text, text, text, text, text, text, text, text, text, text, text, text, text, boolean, text, text, integer, integer, integer, numeric, text, text, boolean, text, text, text, text, numeric, text, text, text, text, text, text, text, text, integer, integer, text, text, text, integer, text);

-- ============================================================
-- STEP 4: Create updated sp_party_create with zone_id
-- ============================================================
CREATE OR REPLACE FUNCTION public.sp_party_create(
    p_company_id integer,
    p_party_type text,
    p_report_party_type text,
    p_salutation text,
    p_first_name text,
    p_last_name text,
    p_company_name text,
    p_display_name text,
    p_email text,
    p_phone_work text,
    p_phone_mobile text,
    p_preferred_language text,
    p_gst_treatment text,
    p_place_of_supply text,
    p_pan text,
    p_gst_no text,
    p_msme_registered boolean,
    p_tax_preference text,
    p_tds text,
    p_currency_id integer,
    p_accounts_receivable_account integer,
    p_accounts_payable_account integer,
    p_opening_balance numeric,
    p_opening_balance_type text,
    p_payment_terms text,
    p_enable_portal boolean,
    p_website_url text,
    p_department text,
    p_remarks text,
    p_party_profile text,
    p_rate_diff numeric,
    p_kam_mobile text,
    p_recovery_mobile text,
    p_sumul_incharge text,
    p_retailer_code text,
    p_sumul_depo_code text,
    p_pan_doc_path text,
    p_aadhaar_doc_path text,
    p_gst_cert_path text,
    p_user_id integer,
    p_salesperson_id integer DEFAULT NULL,
    p_salesperson_mobile text DEFAULT NULL,
    p_kam text DEFAULT NULL,
    p_recovery_agent text DEFAULT NULL,
    p_transporter_id integer DEFAULT NULL,
    p_transporter_mobile text DEFAULT NULL,
    p_zone_id integer DEFAULT NULL
)
RETURNS integer
LANGUAGE plpgsql
AS $function$
DECLARE
    v_party_id INT;
BEGIN
    INSERT INTO tab_party_master (
        company_id, party_type, salutation, first_name, last_name, company_name, display_name,
        email, phone_work, phone_mobile, preferred_language, gst_treatment, place_of_supply,
        pan, gst_no, msme_registered, tax_preference, tds, currency_id,
        accounts_receivable_account, accounts_payable_account, opening_balance,
        opening_balance_type, payment_terms, enable_portal, website_url,
        department, remarks, party_profile, rate_diff, report_party_type,
        kam_mobile, recovery_mobile, sumul_incharge, retailer_code, sumul_depo_code,
        pan_doc_path, aadhaar_doc_path, gst_cert_path,
        salesperson_id, salesperson_mobile, kam, recovery_agent, transporter_id, transporter_mobile,
        zone_id,
        created_on, modified_on
    ) VALUES (
        p_company_id, p_party_type, p_salutation, p_first_name, p_last_name, p_company_name, p_display_name,
        p_email, p_phone_work, p_phone_mobile, p_preferred_language, p_gst_treatment, p_place_of_supply,
        p_pan, p_gst_no, p_msme_registered, p_tax_preference, p_tds, p_currency_id,
        p_accounts_receivable_account, p_accounts_payable_account, p_opening_balance,
        p_opening_balance_type, p_payment_terms, p_enable_portal, p_website_url,
        p_department, p_remarks, p_party_profile, p_rate_diff, COALESCE(p_report_party_type, p_party_type),
        p_kam_mobile, p_recovery_mobile, p_sumul_incharge, p_retailer_code, p_sumul_depo_code,
        p_pan_doc_path, p_aadhaar_doc_path, p_gst_cert_path,
        p_salesperson_id, p_salesperson_mobile, p_kam, p_recovery_agent, p_transporter_id, p_transporter_mobile,
        p_zone_id,
        CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
    ) RETURNING party_id INTO v_party_id;

    RETURN v_party_id;
END;
$function$;

-- ============================================================
-- STEP 5: Create updated sp_party_update with zone_id
-- ============================================================
CREATE OR REPLACE FUNCTION public.sp_party_update(
    p_party_id integer,
    p_company_id integer,
    p_party_type text,
    p_report_party_type text,
    p_salutation text,
    p_first_name text,
    p_last_name text,
    p_company_name text,
    p_display_name text,
    p_email text,
    p_phone_work text,
    p_phone_mobile text,
    p_preferred_language text,
    p_gst_treatment text,
    p_place_of_supply text,
    p_pan text,
    p_gst_no text,
    p_msme_registered boolean,
    p_tax_preference text,
    p_tds text,
    p_currency_id integer,
    p_accounts_receivable_account integer,
    p_accounts_payable_account integer,
    p_opening_balance numeric,
    p_opening_balance_type text,
    p_payment_terms text,
    p_enable_portal boolean,
    p_website_url text,
    p_department text,
    p_remarks text,
    p_party_profile text,
    p_rate_diff numeric,
    p_kam_mobile text,
    p_recovery_mobile text,
    p_sumul_incharge text,
    p_retailer_code text,
    p_sumul_depo_code text,
    p_pan_doc_path text,
    p_aadhaar_doc_path text,
    p_gst_cert_path text,
    p_user_id integer,
    p_salesperson_id integer DEFAULT NULL,
    p_salesperson_mobile text DEFAULT NULL,
    p_kam text DEFAULT NULL,
    p_recovery_agent text DEFAULT NULL,
    p_transporter_id integer DEFAULT NULL,
    p_transporter_mobile text DEFAULT NULL,
    p_zone_id integer DEFAULT NULL
)
RETURNS void
LANGUAGE plpgsql
AS $function$
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
        zone_id = p_zone_id,
        modified_on = CURRENT_TIMESTAMP
    WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$function$;

-- ============================================================
-- STEP 6: Add sp_master_universal support for tab_zone_master
-- (add zone zone_id, zone_name columns to the universal handler)
-- The universal SP handles SELECT/INSERT/UPDATE/DELETE
-- via sp_master_universal which uses the master_name param.
-- We just need to ensure zone master is handled by the universal SP.
-- No changes needed to sp_master_universal if it uses dynamic SQL.
-- ============================================================
