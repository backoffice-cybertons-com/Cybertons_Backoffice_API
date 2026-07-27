-- =============================================================================
-- Migration: Sales Profit Invoice & Party Extended Fields
-- Target: PostgreSQL TENANT database (e.g. softaccdb_1) — NOT companymaster registry DB
-- Check: SELECT connection_string FROM tab_company_master WHERE company_id = YOUR_ID;
--
-- Adds columns for:
--   - Party profile / document paths / Sumul codes
--   - Party bank cancel cheque path
--   - Sales header salesperson
--   - Sales line dealer_price (snapshot of product.selling_price) & sp_comm_unit
--
-- Does NOT add: adj_price (use rate_diff), commission/MRP totals (compute in UI)
-- Dealer price source: tab_product_master.selling_price
-- =============================================================================

BEGIN;

-- -----------------------------------------------------------------------------
-- 1. tab_party_master — customer profile & document paths
-- -----------------------------------------------------------------------------
ALTER TABLE public.tab_party_master ADD COLUMN IF NOT EXISTS kam_mobile VARCHAR(20);
ALTER TABLE public.tab_party_master ADD COLUMN IF NOT EXISTS recovery_mobile VARCHAR(20);
ALTER TABLE public.tab_party_master ADD COLUMN IF NOT EXISTS sumul_incharge VARCHAR(100);
ALTER TABLE public.tab_party_master ADD COLUMN IF NOT EXISTS retailer_code VARCHAR(50);
ALTER TABLE public.tab_party_master ADD COLUMN IF NOT EXISTS sumul_depo_code VARCHAR(50);
ALTER TABLE public.tab_party_master ADD COLUMN IF NOT EXISTS pan_doc_path VARCHAR(500);
ALTER TABLE public.tab_party_master ADD COLUMN IF NOT EXISTS aadhaar_doc_path VARCHAR(500);
ALTER TABLE public.tab_party_master ADD COLUMN IF NOT EXISTS gst_cert_path VARCHAR(500);

COMMENT ON COLUMN public.tab_party_master.kam_mobile IS 'Key account manager mobile';
COMMENT ON COLUMN public.tab_party_master.recovery_mobile IS 'Recovery agent mobile';
COMMENT ON COLUMN public.tab_party_master.sumul_incharge IS 'Sumul area incharge';
COMMENT ON COLUMN public.tab_party_master.retailer_code IS 'Customer retailer code';
COMMENT ON COLUMN public.tab_party_master.sumul_depo_code IS 'Sumul depot code';
COMMENT ON COLUMN public.tab_party_master.pan_doc_path IS 'PAN card document file path';
COMMENT ON COLUMN public.tab_party_master.aadhaar_doc_path IS 'Aadhaar card document file path';
COMMENT ON COLUMN public.tab_party_master.gst_cert_path IS 'GST certificate document file path';

-- -----------------------------------------------------------------------------
-- 2. tab_party_bank_details — cancelled cheque scan
-- -----------------------------------------------------------------------------
ALTER TABLE public.tab_party_bank_details ADD COLUMN IF NOT EXISTS cancel_cheque_path VARCHAR(500);

COMMENT ON COLUMN public.tab_party_bank_details.cancel_cheque_path IS 'Cancelled cheque document file path';

-- -----------------------------------------------------------------------------
-- 3. tab_sale_pur — salesperson on invoice header
-- -----------------------------------------------------------------------------
ALTER TABLE public.tab_sale_pur ADD COLUMN IF NOT EXISTS salesperson_id INT;

COMMENT ON COLUMN public.tab_sale_pur.salesperson_id IS 'FK to tab_salesperson_master';

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'fk_sale_pur_salesperson'
          AND conrelid = 'public.tab_sale_pur'::regclass
    ) THEN
        ALTER TABLE public.tab_sale_pur
            ADD CONSTRAINT fk_sale_pur_salesperson
            FOREIGN KEY (salesperson_id)
            REFERENCES public.tab_salesperson_master (salesperson_id);
    END IF;
EXCEPTION
    WHEN undefined_table THEN
        RAISE NOTICE 'tab_salesperson_master not found — skipping FK fk_sale_pur_salesperson';
END $$;

-- -----------------------------------------------------------------------------
-- 4. tab_sal_pur_tran — unit snapshots for profit invoice
-- -----------------------------------------------------------------------------
ALTER TABLE public.tab_sal_pur_tran ADD COLUMN IF NOT EXISTS dealer_price NUMERIC(18, 4) DEFAULT 0;
ALTER TABLE public.tab_sal_pur_tran ADD COLUMN IF NOT EXISTS sp_comm_unit NUMERIC(18, 4) DEFAULT 0;

COMMENT ON COLUMN public.tab_sal_pur_tran.dealer_price IS 'Snapshot of product.selling_price (dealer price) at save time';
COMMENT ON COLUMN public.tab_sal_pur_tran.sp_comm_unit IS 'Salesperson commission per unit (from tab_add_price.sale_per_comission)';

-- Backfill dealer_price on existing lines where possible
UPDATE public.tab_sal_pur_tran
SET dealer_price = COALESCE(rate, 0) - COALESCE(rate_diff, 0)
WHERE COALESCE(dealer_price, 0) = 0
  AND COALESCE(delete_flag, FALSE) = FALSE;

-- -----------------------------------------------------------------------------
-- 5. sp_party_create / sp_party_update
-- -----------------------------------------------------------------------------
DROP FUNCTION IF EXISTS public.sp_party_create(
    INT, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR,
    VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, BOOLEAN, VARCHAR, VARCHAR, INT, INT, INT,
    NUMERIC, VARCHAR, VARCHAR, BOOLEAN, VARCHAR, VARCHAR, VARCHAR, NUMERIC, VARCHAR, INT
);

CREATE OR REPLACE FUNCTION public.sp_party_create(
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
    p_kam_mobile VARCHAR DEFAULT NULL,
    p_recovery_mobile VARCHAR DEFAULT NULL,
    p_sumul_incharge VARCHAR DEFAULT NULL,
    p_retailer_code VARCHAR DEFAULT NULL,
    p_sumul_depo_code VARCHAR DEFAULT NULL,
    p_pan_doc_path VARCHAR DEFAULT NULL,
    p_aadhaar_doc_path VARCHAR DEFAULT NULL,
    p_gst_cert_path VARCHAR DEFAULT NULL,
    p_user_id INT DEFAULT NULL
)
RETURNS INT AS $$
DECLARE
    v_party_id INT;
BEGIN
    INSERT INTO public.tab_party_master (
        company_id, party_type, salutation, first_name, last_name, company_name, display_name,
        email, phone_work, phone_mobile, preferred_language, gst_treatment, place_of_supply,
        pan, gst_no, msme_registered, tax_preference, tds, currency_id,
        accounts_receivable_account, accounts_payable_account, opening_balance,
        opening_balance_type, payment_terms, enable_portal, website_url,
        department, remarks, party_profile, rate_diff, report_party_type,
        kam_mobile, recovery_mobile, sumul_incharge, retailer_code, sumul_depo_code,
        pan_doc_path, aadhaar_doc_path, gst_cert_path,
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
        CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
    ) RETURNING party_id INTO v_party_id;

    RETURN v_party_id;
END;
$$ LANGUAGE plpgsql;


DROP FUNCTION IF EXISTS public.sp_party_update(
    INT, INT, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR,
    VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, BOOLEAN, VARCHAR, VARCHAR, INT, INT, INT,
    NUMERIC, VARCHAR, VARCHAR, BOOLEAN, VARCHAR, VARCHAR, VARCHAR, NUMERIC, VARCHAR, INT
);

CREATE OR REPLACE FUNCTION public.sp_party_update(
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
    p_kam_mobile VARCHAR DEFAULT NULL,
    p_recovery_mobile VARCHAR DEFAULT NULL,
    p_sumul_incharge VARCHAR DEFAULT NULL,
    p_retailer_code VARCHAR DEFAULT NULL,
    p_sumul_depo_code VARCHAR DEFAULT NULL,
    p_pan_doc_path VARCHAR DEFAULT NULL,
    p_aadhaar_doc_path VARCHAR DEFAULT NULL,
    p_gst_cert_path VARCHAR DEFAULT NULL,
    p_user_id INT DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    UPDATE public.tab_party_master
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
        kam_mobile = p_kam_mobile,
        recovery_mobile = p_recovery_mobile,
        sumul_incharge = p_sumul_incharge,
        retailer_code = p_retailer_code,
        sumul_depo_code = p_sumul_depo_code,
        pan_doc_path = p_pan_doc_path,
        aadhaar_doc_path = p_aadhaar_doc_path,
        gst_cert_path = p_gst_cert_path,
        modified_on = CURRENT_TIMESTAMP
    WHERE party_id = p_party_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- -----------------------------------------------------------------------------
-- 6. sp_party_bank_details_create / sp_party_bank_details_update
-- -----------------------------------------------------------------------------
DROP FUNCTION IF EXISTS public.sp_party_bank_details_create(INT, INT, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, BOOLEAN);
DROP FUNCTION IF EXISTS public.sp_party_bank_details_update(INT, INT, INT, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, VARCHAR, BOOLEAN);

CREATE OR REPLACE FUNCTION public.sp_party_bank_details_create(
    p_company_id INT,
    p_party_id INT,
    p_account_holder_name VARCHAR,
    p_bank_name VARCHAR,
    p_account_number VARCHAR,
    p_ifsc VARCHAR,
    p_branch VARCHAR,
    p_micr VARCHAR,
    p_is_default BOOLEAN,
    p_cancel_cheque_path VARCHAR DEFAULT NULL
)
RETURNS INT AS $$
DECLARE
    v_bank_id INT;
BEGIN
    INSERT INTO public.tab_party_bank_details (
        company_id, party_id, account_holder_name, bank_name, account_number, ifsc,
        branch, micr, is_default, cancel_cheque_path, created_on
    )
    VALUES (
        p_company_id, p_party_id, p_account_holder_name, p_bank_name, p_account_number, p_ifsc,
        p_branch, p_micr, COALESCE(p_is_default, FALSE), p_cancel_cheque_path, CURRENT_TIMESTAMP
    )
    RETURNING bank_id INTO v_bank_id;
    RETURN v_bank_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION public.sp_party_bank_details_update(
    p_bank_id INT,
    p_company_id INT,
    p_party_id INT,
    p_account_holder_name VARCHAR,
    p_bank_name VARCHAR,
    p_account_number VARCHAR,
    p_ifsc VARCHAR,
    p_branch VARCHAR,
    p_micr VARCHAR,
    p_is_default BOOLEAN,
    p_cancel_cheque_path VARCHAR DEFAULT NULL
)
RETURNS VOID AS $$
BEGIN
    UPDATE public.tab_party_bank_details
    SET
        party_id = p_party_id,
        account_holder_name = p_account_holder_name,
        bank_name = p_bank_name,
        account_number = p_account_number,
        ifsc = p_ifsc,
        branch = p_branch,
        micr = p_micr,
        is_default = COALESCE(p_is_default, FALSE),
        cancel_cheque_path = p_cancel_cheque_path
    WHERE bank_id = p_bank_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- -----------------------------------------------------------------------------
-- 7. fn_sale_pur — add p_salesperson_id
-- -----------------------------------------------------------------------------
DROP FUNCTION IF EXISTS public.fn_sale_pur(
    character varying, integer, integer, character varying, integer, character varying,
    character varying, character varying, character varying, date, character varying, date,
    integer, integer, date, character varying, integer, integer, integer, integer, integer,
    numeric, numeric, text, character varying, character varying, text, character varying,
    character varying, numeric, integer, character varying, date, date, integer, integer,
    text, numeric, numeric, numeric, numeric, numeric, numeric, numeric, numeric, numeric,
    numeric, numeric, character varying, integer, integer, integer, integer, integer,
    boolean, numeric, numeric, numeric, numeric, integer
);

CREATE OR REPLACE FUNCTION public.fn_sale_pur(
    p_action character varying,
    p_sp_unique_code integer DEFAULT NULL::integer,
    p_company_id integer DEFAULT NULL::integer,
    p_invoice_series character varying DEFAULT NULL::character varying,
    p_invoice_series_increment_number integer DEFAULT NULL::integer,
    p_inv_type character varying DEFAULT NULL::character varying,
    p_inv_sub_type character varying DEFAULT NULL::character varying,
    p_inv_category character varying DEFAULT NULL::character varying,
    p_sales_order_no character varying DEFAULT NULL::character varying,
    p_sales_order_date date DEFAULT NULL::date,
    p_po_order_no character varying DEFAULT NULL::character varying,
    p_po_order_date date DEFAULT NULL::date,
    p_bill_id integer DEFAULT NULL::integer,
    p_delivery_no integer DEFAULT NULL::integer,
    p_delivery_date date DEFAULT NULL::date,
    p_vehicle_no character varying DEFAULT NULL::character varying,
    p_transporter_id integer DEFAULT NULL::integer,
    p_unit_id integer DEFAULT NULL::integer,
    p_bank_id integer DEFAULT NULL::integer,
    p_product_id integer DEFAULT NULL::integer,
    p_tax_id integer DEFAULT NULL::integer,
    p_net_invoice_amount numeric DEFAULT NULL::numeric,
    p_grand_total numeric DEFAULT NULL::numeric,
    p_billing_address text DEFAULT NULL::text,
    p_billing_address1 character varying DEFAULT NULL::character varying,
    p_billing_address2 character varying DEFAULT NULL::character varying,
    p_shipping_address text DEFAULT NULL::text,
    p_shipping_address1 character varying DEFAULT NULL::character varying,
    p_shipping_address2 character varying DEFAULT NULL::character varying,
    p_tot_discount numeric DEFAULT 0,
    p_tot_sku_in_invoice integer DEFAULT 0,
    p_inv_no character varying DEFAULT NULL::character varying,
    p_inv_date date DEFAULT NULL::date,
    p_due_date date DEFAULT NULL::date,
    p_party_id integer DEFAULT NULL::integer,
    p_terms_id integer DEFAULT NULL::integer,
    p_description text DEFAULT NULL::text,
    p_taxable_amount numeric DEFAULT NULL::numeric,
    p_igst_percent numeric DEFAULT NULL::numeric,
    p_igst_amount numeric DEFAULT NULL::numeric,
    p_cgst_percent numeric DEFAULT NULL::numeric,
    p_cgst_amount numeric DEFAULT NULL::numeric,
    p_sgst_percent numeric DEFAULT NULL::numeric,
    p_sgst_amount numeric DEFAULT NULL::numeric,
    p_round_off numeric DEFAULT NULL::numeric,
    p_previous_outstanding numeric DEFAULT NULL::numeric,
    p_current_bill_amount numeric DEFAULT NULL::numeric,
    p_final_outstanding numeric DEFAULT NULL::numeric,
    p_jurisdiction character varying DEFAULT NULL::character varying,
    p_fiscal_year_id integer DEFAULT NULL::integer,
    p_branch_id integer DEFAULT NULL::integer,
    p_created_by integer DEFAULT NULL::integer,
    p_modified_by integer DEFAULT NULL::integer,
    p_deleted_by integer DEFAULT NULL::integer,
    p_delete_flag boolean DEFAULT false,
    p_rate_diff numeric DEFAULT 0,
    p_invoice_total numeric DEFAULT 0,
    p_final_invoice_amount numeric DEFAULT 0,
    p_total_quantity numeric DEFAULT 0,
    p_salesperson_id integer DEFAULT NULL::integer,
    p_user_id integer DEFAULT NULL::integer)
RETURNS SETOF public.tab_sale_pur
LANGUAGE plpgsql
AS $BODY$
DECLARE
    v_audit_user INT;
    v_tab_id INT;
    v_perm_action VARCHAR(20);
BEGIN
    v_audit_user := COALESCE(p_user_id, p_modified_by, p_deleted_by, p_created_by);

    v_tab_id := CASE
        WHEN UPPER(TRIM(COALESCE(p_inv_type, ''))) = 'PURCHASE' THEN 26
        ELSE 16
    END;

    v_perm_action := CASE
        WHEN p_action = 'INSERT' THEN 'ADD'
        WHEN p_action = 'UPDATE' THEN 'EDIT'
        ELSE p_action
    END;

    IF v_audit_user IS NOT NULL AND v_perm_action IN ('ADD', 'EDIT', 'DELETE') THEN
        IF NOT public.fn_validate_permission(v_audit_user, p_company_id, v_tab_id, v_perm_action) THEN
            RAISE EXCEPTION 'Access Denied: Module % (Tab %).', v_perm_action, v_tab_id;
        END IF;
    END IF;

    IF p_action = 'INSERT' THEN
        RETURN QUERY
        INSERT INTO public.tab_sale_pur (
            company_id, invoice_series, invoice_series_increment_number, inv_type, inv_sub_type, inv_category,
            sales_order_no, sales_order_date, po_order_no, po_order_date, bill_id, delivery_no, delivery_date,
            vehicle_no, transporter_id, unit_id, bank_id, product_id, tax_id, net_invoice_amount, grand_total,
            billing_address, billing_address1, billing_address2, shipping_address, shipping_address1, shipping_address2,
            tot_discount, tot_sku_in_invoice, inv_no, inv_date, due_date, party_id, terms_id, description,
            taxable_amount, igst_percent, igst_amount, cgst_percent, cgst_amount, sgst_percent, sgst_amount,
            round_off, previous_outstanding, current_bill_amount, final_outstanding, jurisdiction,
            fiscal_year_id, branch_id, created_by, created_on, delete_flag,
            rate_diff, invoice_total, final_invoice_amount, total_quantity, salesperson_id
        )
        VALUES (
            p_company_id, p_invoice_series, p_invoice_series_increment_number, p_inv_type, p_inv_sub_type, p_inv_category,
            p_sales_order_no, p_sales_order_date, p_po_order_no, p_po_order_date, p_bill_id, p_delivery_no, p_delivery_date,
            p_vehicle_no, p_transporter_id, p_unit_id, p_bank_id, p_product_id, p_tax_id, p_net_invoice_amount, p_grand_total,
            p_billing_address, p_billing_address1, p_billing_address2, p_shipping_address, p_shipping_address1, p_shipping_address2,
            p_tot_discount, p_tot_sku_in_invoice, p_inv_no, p_inv_date, p_due_date, p_party_id, p_terms_id, p_description,
            p_taxable_amount, p_igst_percent, p_igst_amount, p_cgst_percent, p_cgst_amount, p_sgst_percent, p_sgst_amount,
            p_round_off, p_previous_outstanding, p_current_bill_amount, p_final_outstanding, p_jurisdiction,
            p_fiscal_year_id, p_branch_id, v_audit_user, CURRENT_TIMESTAMP, p_delete_flag,
            p_rate_diff, p_invoice_total, p_final_invoice_amount, p_total_quantity, p_salesperson_id
        )
        RETURNING *;

    ELSIF p_action = 'UPDATE' THEN
        UPDATE public.tab_sale_pur SET
            invoice_series = p_invoice_series,
            invoice_series_increment_number = p_invoice_series_increment_number,
            inv_sub_type = p_inv_sub_type,
            inv_category = p_inv_category,
            sales_order_no = p_sales_order_no,
            sales_order_date = p_sales_order_date,
            po_order_no = p_po_order_no,
            po_order_date = p_po_order_date,
            bill_id = p_bill_id,
            delivery_no = p_delivery_no,
            delivery_date = p_delivery_date,
            vehicle_no = p_vehicle_no,
            transporter_id = p_transporter_id,
            unit_id = p_unit_id,
            bank_id = p_bank_id,
            product_id = p_product_id,
            tax_id = p_tax_id,
            net_invoice_amount = p_net_invoice_amount,
            grand_total = p_grand_total,
            billing_address = p_billing_address,
            billing_address1 = p_billing_address1,
            billing_address2 = p_billing_address2,
            shipping_address = p_shipping_address,
            shipping_address1 = p_shipping_address1,
            shipping_address2 = p_shipping_address2,
            tot_discount = p_tot_discount,
            tot_sku_in_invoice = p_tot_sku_in_invoice,
            inv_no = p_inv_no,
            inv_date = p_inv_date,
            due_date = p_due_date,
            party_id = p_party_id,
            terms_id = p_terms_id,
            description = p_description,
            taxable_amount = p_taxable_amount,
            igst_percent = p_igst_percent,
            igst_amount = p_igst_amount,
            cgst_percent = p_cgst_percent,
            cgst_amount = p_cgst_amount,
            sgst_percent = p_sgst_percent,
            sgst_amount = p_sgst_amount,
            round_off = p_round_off,
            previous_outstanding = p_previous_outstanding,
            current_bill_amount = p_current_bill_amount,
            final_outstanding = p_final_outstanding,
            jurisdiction = p_jurisdiction,
            fiscal_year_id = p_fiscal_year_id,
            branch_id = p_branch_id,
            modified_by = v_audit_user,
            modified_on = CURRENT_TIMESTAMP,
            delete_flag = p_delete_flag,
            rate_diff = p_rate_diff,
            invoice_total = p_invoice_total,
            final_invoice_amount = p_final_invoice_amount,
            total_quantity = p_total_quantity,
            salesperson_id = p_salesperson_id
        WHERE sp_unique_code = p_sp_unique_code AND company_id = p_company_id;

        RETURN QUERY SELECT * FROM public.tab_sale_pur WHERE sp_unique_code = p_sp_unique_code;

    ELSIF p_action = 'DELETE' THEN
        RETURN QUERY
        UPDATE public.tab_sale_pur
        SET delete_flag = TRUE,
            deleted_by = v_audit_user,
            deleted_on = CURRENT_TIMESTAMP
        WHERE sp_unique_code = p_sp_unique_code
          AND company_id = p_company_id
        RETURNING *;

    ELSIF p_action = 'GETALL' THEN
        RETURN QUERY
        SELECT * FROM public.tab_sale_pur
        WHERE company_id = p_company_id
          AND delete_flag = FALSE
        ORDER BY sp_unique_code DESC;

    ELSIF p_action = 'GETBYID' THEN
        RETURN QUERY
        SELECT * FROM public.tab_sale_pur
        WHERE sp_unique_code = p_sp_unique_code
          AND company_id = p_company_id;
    END IF;
END;
$BODY$;

-- -----------------------------------------------------------------------------
-- 8. fn_sal_pur_tran — add p_dealer_price, p_sp_comm_unit
-- -----------------------------------------------------------------------------
DROP FUNCTION IF EXISTS public.fn_sal_pur_tran(
    character varying, integer, integer, integer, integer, text, date, character varying,
    numeric, integer, numeric, integer, numeric, numeric, numeric, numeric, numeric, numeric,
    numeric, numeric, numeric, numeric, numeric, numeric, character varying, character varying,
    integer, integer, integer, integer, integer, boolean, numeric, numeric
);

CREATE OR REPLACE FUNCTION public.fn_sal_pur_tran(
    p_action character varying,
    p_sp_tran_unique_code integer DEFAULT NULL::integer,
    p_sp_unique_code integer DEFAULT NULL::integer,
    p_company_id integer DEFAULT NULL::integer,
    p_product_id integer DEFAULT NULL::integer,
    p_product_details text DEFAULT NULL::text,
    p_batch_date date DEFAULT NULL::date,
    p_batch_no character varying DEFAULT NULL::character varying,
    p_mrp numeric DEFAULT NULL::numeric,
    p_sub_unit integer DEFAULT NULL::integer,
    p_sub_unit_quantity numeric DEFAULT NULL::numeric,
    p_unit integer DEFAULT NULL::integer,
    p_quantity numeric DEFAULT NULL::numeric,
    p_rate numeric DEFAULT NULL::numeric,
    p_discount_per numeric DEFAULT 0,
    p_amount numeric DEFAULT NULL::numeric,
    p_igst_percent numeric DEFAULT NULL::numeric,
    p_igst_amount numeric DEFAULT NULL::numeric,
    p_cgst_percent numeric DEFAULT NULL::numeric,
    p_cgst_amount numeric DEFAULT NULL::numeric,
    p_sgst_percent numeric DEFAULT NULL::numeric,
    p_sgst_amount numeric DEFAULT NULL::numeric,
    p_total_amount numeric DEFAULT NULL::numeric,
    p_crt numeric DEFAULT NULL::numeric,
    p_packing character varying DEFAULT NULL::character varying,
    p_hsn_code character varying DEFAULT NULL::character varying,
    p_fiscal_year_id integer DEFAULT NULL::integer,
    p_branch_id integer DEFAULT NULL::integer,
    p_created_by integer DEFAULT NULL::integer,
    p_modified_by integer DEFAULT NULL::integer,
    p_deleted_by integer DEFAULT NULL::integer,
    p_delete_flag boolean DEFAULT false,
    p_rate_diff numeric DEFAULT 0,
    p_cogs numeric DEFAULT 0,
    p_dealer_price numeric DEFAULT 0,
    p_sp_comm_unit numeric DEFAULT 0)
RETURNS SETOF public.tab_sal_pur_tran
LANGUAGE plpgsql
AS $BODY$
DECLARE
    v_audit_user INT;
BEGIN
    v_audit_user := COALESCE(p_modified_by, p_deleted_by, p_created_by);

    IF p_action = 'INSERT' THEN
        RETURN QUERY
        INSERT INTO public.tab_sal_pur_tran (
            sp_unique_code, company_id, product_id, product_details, batch_date, batch_no, mrp,
            sub_unit, sub_unit_quantity, unit, quantity, rate, discount_per, amount,
            igst_percent, igst_amount, cgst_percent, cgst_amount, sgst_percent, sgst_amount,
            total_amount, crt, packing, hsn_code, fiscal_year_id, branch_id, created_by, created_on,
            modified_by, modified_on, delete_flag, rate_diff, cogs, dealer_price, sp_comm_unit
        )
        VALUES (
            p_sp_unique_code, p_company_id, p_product_id, p_product_details, p_batch_date, p_batch_no, p_mrp,
            p_sub_unit, p_sub_unit_quantity, p_unit, p_quantity, p_rate, p_discount_per, p_amount,
            p_igst_percent, p_igst_amount, p_cgst_percent, p_cgst_amount, p_sgst_percent, p_sgst_amount,
            p_total_amount, p_crt, p_packing, p_hsn_code, p_fiscal_year_id, p_branch_id,
            COALESCE(p_created_by, v_audit_user), CURRENT_TIMESTAMP,
            v_audit_user, CURRENT_TIMESTAMP, p_delete_flag, p_rate_diff, p_cogs, p_dealer_price, p_sp_comm_unit
        )
        RETURNING *;

    ELSIF p_action = 'UPDATE' THEN
        UPDATE public.tab_sal_pur_tran SET
            product_id = p_product_id,
            product_details = p_product_details,
            batch_date = p_batch_date,
            batch_no = p_batch_no,
            mrp = p_mrp,
            sub_unit = p_sub_unit,
            sub_unit_quantity = p_sub_unit_quantity,
            unit = p_unit,
            quantity = p_quantity,
            rate = p_rate,
            discount_per = p_discount_per,
            amount = p_amount,
            total_amount = p_total_amount,
            rate_diff = p_rate_diff,
            cogs = p_cogs,
            dealer_price = p_dealer_price,
            sp_comm_unit = p_sp_comm_unit,
            modified_by = v_audit_user,
            modified_on = CURRENT_TIMESTAMP
        WHERE sp_tran_unique_code = p_sp_tran_unique_code;

        RETURN QUERY SELECT * FROM public.tab_sal_pur_tran WHERE sp_tran_unique_code = p_sp_tran_unique_code;

    ELSIF p_action = 'DELETE' THEN
        UPDATE public.tab_sal_pur_tran SET
            delete_flag = TRUE,
            deleted_by = v_audit_user,
            deleted_on = CURRENT_TIMESTAMP,
            modified_by = v_audit_user,
            modified_on = CURRENT_TIMESTAMP
        WHERE sp_tran_unique_code = p_sp_tran_unique_code;

    ELSIF p_action = 'DELETE_BY_PARENT' THEN
        UPDATE public.tab_sal_pur_tran SET
            delete_flag = TRUE,
            deleted_by = v_audit_user,
            deleted_on = CURRENT_TIMESTAMP,
            modified_by = v_audit_user,
            modified_on = CURRENT_TIMESTAMP
        WHERE sp_unique_code = p_sp_unique_code;

    ELSIF p_action = 'GETBYPARENT' THEN
        RETURN QUERY
        SELECT * FROM public.tab_sal_pur_tran
        WHERE sp_unique_code = p_sp_unique_code
          AND delete_flag = FALSE;
    END IF;
END;
$BODY$;

COMMIT;

-- =============================================================================
-- Post-migration verification (run manually):
--
-- SELECT column_name FROM information_schema.columns
-- WHERE table_name = 'tab_party_master'
--   AND column_name IN ('kam_mobile','recovery_mobile','sumul_incharge',
--                       'retailer_code','sumul_depo_code','pan_doc_path',
--                       'aadhaar_doc_path','gst_cert_path');
--
-- SELECT column_name FROM information_schema.columns
-- WHERE table_name = 'tab_sal_pur_tran'
--   AND column_name IN ('dealer_price','sp_comm_unit');
--
-- SELECT column_name FROM information_schema.columns
-- WHERE table_name = 'tab_sale_pur' AND column_name = 'salesperson_id';
-- =============================================================================
