-----------------------------------------------------------
-- POSTGRESQL MIGRATION SCRIPT FOR SoftAccDb
-- Converted from SQL Server to PostgreSQL
-- Multi-Tenant Architecture: CompanyId added to all tables
-- Database: SoftAccDb
-- Convention: snake_case for PostgreSQL compatibility
-----------------------------------------------------------

-- IMPORTANT NOTES:
-- 1. tab_company_master is in a SEPARATE database (CompanyMaster)
--    Therefore, FOREIGN KEY constraints to tab_company_master CANNOT be created
--    CompanyId validation must be done at APPLICATION LEVEL
-- 2. All tables have CompanyId column for multi-tenant filtering
-- 3. All tables have indexes on CompanyId for performance
-- 4. Use Row-Level Security (RLS) for automatic CompanyId filtering (recommended)
-- 5. NOTE: This script includes FK constraints that will FAIL - remove them manually
--    OR run REMOVE_COMPANY_FK.sql after table creation
--    Search for: "CONSTRAINT fk_*_company FOREIGN KEY (company_id) REFERENCES tab_company_master"

-- Enable UUID extension if needed
-- CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-----------------------------------------------------------
-- SCHEMA SETUP
-----------------------------------------------------------
CREATE SCHEMA IF NOT EXISTS public;
SET search_path TO public;

-----------------------------------------------------------
-- TABLE: TabAccountMaster
-- Multi-tenant: CompanyId added for filtering
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_account_master (
    account_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    account_type VARCHAR(100),
    account_name VARCHAR(200),
    is_sub_account BOOLEAN NOT NULL DEFAULT FALSE,
    parent_account_id INT,
    account_code VARCHAR(50),
    description VARCHAR(500),
    CONSTRAINT fk_account_parent FOREIGN KEY (parent_account_id) REFERENCES tab_account_master(account_id)
    -- Note: company_id FK removed - tab_company_master is in different database
    -- Application must validate company_id exists in CompanyMaster database
);

CREATE INDEX IF NOT EXISTS idx_account_company ON tab_account_master(company_id);
CREATE INDEX IF NOT EXISTS idx_account_parent ON tab_account_master(parent_account_id);

-----------------------------------------------------------
-- TABLE: TabAddressTypeMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_address_type_master (
    address_type_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    address_type_code VARCHAR(30) NOT NULL,
    address_type_name VARCHAR(100) NOT NULL,
    usage_category VARCHAR(30) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    sort_order INT NOT NULL DEFAULT 1,
    created_on TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
)

CREATE INDEX IF NOT EXISTS idx_address_type_company ON tab_address_type_master(company_id);

-----------------------------------------------------------
-- TABLE: TabBankMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_bank_master (
    bank_master_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    bank_name VARCHAR(200) NOT NULL,
    branch VARCHAR(150),
    account_number VARCHAR(50),
    ifsc VARCHAR(20),
    micr VARCHAR(20)
)

CREATE INDEX IF NOT EXISTS idx_bank_company ON tab_bank_master(company_id);

-----------------------------------------------------------
-- TABLE: TabBill
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_bill (
    bill_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    vendor_id INT NOT NULL,
    location_id INT NOT NULL,
    bill_number VARCHAR(20) NOT NULL,
    order_number VARCHAR(50),
    bill_date DATE NOT NULL,
    due_date DATE NOT NULL,
    payment_terms VARCHAR(50) NOT NULL,
    accounts_payable_id INT NOT NULL,
    reverse_charge BOOLEAN,
    subject VARCHAR(300),
    customer_notes VARCHAR(500),
    terms TEXT,

    CONSTRAINT fk_bill_vendor FOREIGN KEY (vendor_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_bill_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id),
    CONSTRAINT fk_bill_account FOREIGN KEY (accounts_payable_id) REFERENCES tab_account_master(account_id)
);

CREATE INDEX IF NOT EXISTS idx_bill_company ON tab_bill(company_id);
CREATE INDEX IF NOT EXISTS idx_bill_vendor ON tab_bill(vendor_id);
CREATE INDEX IF NOT EXISTS idx_bill_location ON tab_bill(location_id);

-----------------------------------------------------------
-- TABLE: TabBillItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_bill_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    bill_id INT,
    item_details VARCHAR(200),
    account_id INT,
    quantity DECIMAL(18,4),
    rate DECIMAL(18, 2),
    discount_pct DECIMAL(5, 2),
    tax_id INT,
    customer_id INT,
    amount DECIMAL(18, 2),
    tds_tcs VARCHAR(5),
    adjustment DECIMAL(18, 2),

    CONSTRAINT fk_bill_items_bill FOREIGN KEY (bill_id) REFERENCES tab_bill(bill_id),
    CONSTRAINT fk_bill_items_account FOREIGN KEY (account_id) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_bill_items_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id),
    CONSTRAINT fk_bill_items_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id)
);

CREATE INDEX IF NOT EXISTS idx_bill_items_company ON tab_bill_items(company_id);
CREATE INDEX IF NOT EXISTS idx_bill_items_bill ON tab_bill_items(bill_id);

-----------------------------------------------------------
-- TABLE: TabBillPayment
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_bill_payment (
    bill_payment_id INT,
    company_id INT NOT NULL,
    vendor_name VARCHAR(200),
    location VARCHAR(100),
    payment_number VARCHAR(50),
    payment_made DECIMAL(18, 2),
    payment_date DATE,
    payment_mode VARCHAR(100),
    paid_through VARCHAR(200),
    reference_number VARCHAR(200),
    notes TEXT
)

CREATE INDEX IF NOT EXISTS idx_bill_payment_company ON tab_bill_payment(company_id);

-----------------------------------------------------------
-- TABLE: TabBranchMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_branch_master (
    branch_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    branch_name VARCHAR(150) NOT NULL,
    address VARCHAR(300),
    city VARCHAR(100),
    state VARCHAR(100),
    country VARCHAR(100),
    phone VARCHAR(20),
    is_active BOOLEAN DEFAULT TRUE
)

CREATE INDEX IF NOT EXISTS idx_branch_company ON tab_branch_master(company_id);

-----------------------------------------------------------
-- TABLE: TabBudget
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_budget (
    budget_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    name VARCHAR(150) NOT NULL,
    fiscal_year VARCHAR(20) NOT NULL,
    budget_period VARCHAR(20) NOT NULL,
    location_id INT,
    include_asset_liability_equity BOOLEAN,
    created_at TIMESTAMP,

    CONSTRAINT fk_budget_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id)
);

CREATE INDEX IF NOT EXISTS idx_budget_company ON tab_budget(company_id);

-----------------------------------------------------------
-- TABLE: TabCostCenterMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_cost_center_master (
    cost_center_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    cost_center_name VARCHAR(150) NOT NULL,
    description VARCHAR(300)
)

CREATE INDEX IF NOT EXISTS idx_cost_center_company ON tab_cost_center_master(company_id);

-----------------------------------------------------------
-- TABLE: TabCreditNotes
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_credit_notes (
    credit_note_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_id INT NOT NULL,
    location_id INT,
    reason VARCHAR(200),
    credit_note_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    credit_note_date DATE NOT NULL,
    salesperson_id INT,
    notes VARCHAR(500),
    terms TEXT,
    subject TEXT,

    CONSTRAINT fk_credit_notes_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_credit_notes_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id)
);

CREATE INDEX IF NOT EXISTS idx_credit_notes_company ON tab_credit_notes(company_id);
CREATE INDEX IF NOT EXISTS idx_credit_notes_customer ON tab_credit_notes(customer_id);

-----------------------------------------------------------
-- TABLE: TabCreditNotesItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_credit_notes_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    credit_note_id INT,
    item_details VARCHAR(200),
    account_id INT,
    quantity DECIMAL(18,4),
    rate DECIMAL(18, 2),
    tax_id INT,
    amount DECIMAL(18, 2),
    discount_pct DECIMAL(5, 2),
    tds_tcs VARCHAR(5),

    CONSTRAINT fk_credit_notes_items_credit_note FOREIGN KEY (credit_note_id) REFERENCES tab_credit_notes(credit_note_id),
    CONSTRAINT fk_credit_notes_items_account FOREIGN KEY (account_id) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_credit_notes_items_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id)
);

CREATE INDEX IF NOT EXISTS idx_credit_notes_items_company ON tab_credit_notes_items(company_id);

-----------------------------------------------------------
-- TABLE: TabCurrencyAdjustment
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_currency_adjustment (
    adjustment_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    currency_id INT NOT NULL,
    adjustment_date DATE NOT NULL,
    exchange_rate DECIMAL(18, 6) NOT NULL,
    to_currency VARCHAR(10) NOT NULL,
    notes VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_currency_adjustment_currency FOREIGN KEY (currency_id) REFERENCES tab_currency_master(currency_id)
);

CREATE INDEX IF NOT EXISTS idx_currency_adjustment_company ON tab_currency_adjustment(company_id);

-----------------------------------------------------------
-- TABLE: TabCurrencyMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_currency_master (
    currency_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    currency_code VARCHAR(10),
    currency_symbol VARCHAR(10),
    currency_name VARCHAR(100),
    decimal_places SMALLINT,
    format VARCHAR(50)
)

CREATE INDEX IF NOT EXISTS idx_currency_company ON tab_currency_master(company_id);

-----------------------------------------------------------
-- TABLE: TabCustomerAdvance
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_customer_advance (
    customer_advance_id INT,
    company_id INT NOT NULL,
    customer_name VARCHAR(200),
    place_of_supply VARCHAR(100),
    location VARCHAR(100),
    description_of_supply TEXT,
    amount_received DECIMAL(18, 2),
    bank_charges DECIMAL(18, 2),
    tax VARCHAR(100),
    payment_date DATE,
    payment_number VARCHAR(50),
    payment_mode VARCHAR(100),
    deposit_to VARCHAR(200),
    reference_number VARCHAR(200),
    notes TEXT
)

CREATE INDEX IF NOT EXISTS idx_customer_advance_company ON tab_customer_advance(company_id);

-----------------------------------------------------------
-- TABLE: TabDeliveryChallan
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_delivery_challan (
    delivery_challan_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_id INT NOT NULL,
    location_id INT,
    challan_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    challan_date DATE NOT NULL,
    challan_type VARCHAR(50) NOT NULL,
    customer_notes VARCHAR(500),
    terms TEXT,
    warehouse_location VARCHAR(250),

    CONSTRAINT fk_delivery_challan_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_delivery_challan_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id)
);

CREATE INDEX IF NOT EXISTS idx_delivery_challan_company ON tab_delivery_challan(company_id);

-----------------------------------------------------------
-- TABLE: TabDeliveryChallanItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_delivery_challan_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    delivery_challan_id INT,
    item_details VARCHAR(200),
    quantity DECIMAL(18,4) NOT NULL,
    rate DECIMAL(18, 2) NOT NULL,
    tax_id INT,
    amount DECIMAL(18, 2) NOT NULL,
    discount_pct DECIMAL(5, 2),
    adjustment DECIMAL(18, 2),

    CONSTRAINT fk_delivery_challan_items_challan FOREIGN KEY (delivery_challan_id) REFERENCES tab_delivery_challan(delivery_challan_id),
    CONSTRAINT fk_delivery_challan_items_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id)
);

CREATE INDEX IF NOT EXISTS idx_delivery_challan_items_company ON tab_delivery_challan_items(company_id);

-----------------------------------------------------------
-- TABLE: TabEmployee
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_employee (
    employee_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    employee_name VARCHAR(200),
    email VARCHAR(200),
    phone VARCHAR(20),
    department VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE
)

CREATE INDEX IF NOT EXISTS idx_employee_company ON tab_employee(company_id);

-----------------------------------------------------------
-- TABLE: TabEWayBills
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_eway_bills (
    eway_bill_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    document_type VARCHAR(50) NOT NULL,
    transaction_type VARCHAR(50) NOT NULL,
    transaction_sub_type VARCHAR(50) NOT NULL,
    location_id INT,
    customer_id INT,
    invoice_no VARCHAR(20) NOT NULL,
    date DATE NOT NULL,
    dispatch_from VARCHAR(200),
    place_of_delivery VARCHAR(200),
    distance_km INT NOT NULL,
    transporter_id INT,
    vehicle_type VARCHAR(50) NOT NULL,
    vehicle_no VARCHAR(50),
    transporter_doc_no VARCHAR(50),
    transporter_doc_date DATE,

    CONSTRAINT fk_eway_bills_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_eway_bills_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id),
    CONSTRAINT fk_eway_bills_transporter FOREIGN KEY (transporter_id) REFERENCES tab_transporter_master(transporter_id)
);

CREATE INDEX IF NOT EXISTS idx_eway_bills_company ON tab_eway_bills(company_id);

-----------------------------------------------------------
-- TABLE: TabEWayBillsItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_eway_bills_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    eway_bill_id INT,
    item_details VARCHAR(200),
    hsn_code INT,
    quantity DECIMAL(18,4),
    taxable_amount DECIMAL(18, 2),
    igst INT,
    cess DECIMAL(18, 2),

    CONSTRAINT fk_eway_bills_items_eway_bill FOREIGN KEY (eway_bill_id) REFERENCES tab_eway_bills(eway_bill_id),
    CONSTRAINT fk_eway_bills_items_hsn FOREIGN KEY (hsn_code) REFERENCES tab_hsn_master(hsn_id),
    CONSTRAINT fk_eway_bills_items_igst FOREIGN KEY (igst) REFERENCES tab_tax_master(tax_id)
);

CREATE INDEX IF NOT EXISTS idx_eway_bills_items_company ON tab_eway_bills_items(company_id);

-----------------------------------------------------------
-- TABLE: TabFiscalYearMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_fiscal_year_master (
    fiscal_year_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    is_current BOOLEAN DEFAULT FALSE
)

CREATE INDEX IF NOT EXISTS idx_fiscal_year_company ON tab_fiscal_year_master(company_id);

-----------------------------------------------------------
-- TABLE: TabGodownMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_godown_master (
    godown_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    warehouse_id INT,
    godown_name VARCHAR(150),

    CONSTRAINT fk_godown_warehouse FOREIGN KEY (warehouse_id) REFERENCES tab_warehouse_master(warehouse_id)
);

CREATE INDEX IF NOT EXISTS idx_godown_company ON tab_godown_master(company_id);

-----------------------------------------------------------
-- TABLE: TabHSNMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_hsn_master (
    hsn_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    hsn_code VARCHAR(20),
    description VARCHAR(300),
    tax_rate DECIMAL(5, 2)
)

CREATE INDEX IF NOT EXISTS idx_hsn_company ON tab_hsn_master(company_id);

-----------------------------------------------------------
-- TABLE: TabInvoice
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_invoice (
    invoice_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_id INT NOT NULL,
    location_id INT,
    invoice_number VARCHAR(20) NOT NULL,
    order_number VARCHAR(50),
    invoice_date DATE NOT NULL,
    due_date DATE,
    payment_terms VARCHAR(50) NOT NULL,
    accounts_receivable INT,
    salesperson_id INT,
    subject VARCHAR(300),
    customer_notes VARCHAR(500),
    terms TEXT,
    warehouse_location VARCHAR(250),

    CONSTRAINT fk_invoice_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_invoice_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id),
    CONSTRAINT fk_invoice_account FOREIGN KEY (accounts_receivable) REFERENCES tab_account_master(account_id)
);

CREATE INDEX IF NOT EXISTS idx_invoice_company ON tab_invoice(company_id);
CREATE INDEX IF NOT EXISTS idx_invoice_customer ON tab_invoice(customer_id);

-----------------------------------------------------------
-- TABLE: TabInvoiceItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_invoice_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    invoice_id INT,
    item_details VARCHAR(200),
    quantity DECIMAL(18,4) NOT NULL,
    rate DECIMAL(18, 2) NOT NULL,
    tax_id INT,
    amount DECIMAL(18, 2) NOT NULL,
    discount_pct DECIMAL(5, 2),
    tds_tcs VARCHAR(5),
    adjustment DECIMAL(18, 2),

    CONSTRAINT fk_invoice_items_invoice FOREIGN KEY (invoice_id) REFERENCES tab_invoice(invoice_id),
    CONSTRAINT fk_invoice_items_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id)
);

CREATE INDEX IF NOT EXISTS idx_invoice_items_company ON tab_invoice_items(company_id);

-----------------------------------------------------------
-- TABLE: TabInvoicePayment
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_invoice_payment (
    invoice_payment_id INT,
    company_id INT NOT NULL,
    customer_name VARCHAR(200),
    location VARCHAR(100),
    amount_received DECIMAL(18, 2),
    bank_charges DECIMAL(18, 2),
    payment_date DATE,
    payment_number VARCHAR(50),
    payment_mode VARCHAR(100),
    deposit_to VARCHAR(200),
    reference_number VARCHAR(200),
    tax_deducted BOOLEAN,
    tds_tax_amount_id INT,
    notes TEXT,
    reference_no VARCHAR(50),
    notes_text VARCHAR(500)
)

CREATE INDEX IF NOT EXISTS idx_invoice_payment_company ON tab_invoice_payment(company_id);

-----------------------------------------------------------
-- TABLE: TabJournal
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_journal (
    journal_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    location_id INT,
    journal_date DATE NOT NULL,
    journal_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    notes VARCHAR(500),
    reporting_method VARCHAR(20) NOT NULL,
    currency VARCHAR(10) NOT NULL,
    attachments TEXT,

    CONSTRAINT fk_journal_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id)
);

CREATE INDEX IF NOT EXISTS idx_journal_company ON tab_journal(company_id);

-----------------------------------------------------------
-- TABLE: TabJournalLines
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_journal_lines (
    line_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    journal_id INT,
    account_id INT,
    description VARCHAR(200),
    contact_id INT,
    debit DECIMAL(18, 2),
    credit DECIMAL(18, 2),

    CONSTRAINT fk_journal_lines_journal FOREIGN KEY (journal_id) REFERENCES tab_journal(journal_id),
    CONSTRAINT fk_journal_lines_account FOREIGN KEY (account_id) REFERENCES tab_account_master(account_id)
);

CREATE INDEX IF NOT EXISTS idx_journal_lines_company ON tab_journal_lines(company_id);

-----------------------------------------------------------
-- TABLE: TabLedgerSettings
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_ledger_settings (
    setting_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    auto_numbering BOOLEAN,
    default_currency VARCHAR(10),
    enable_tcs BOOLEAN,
    enable_tds BOOLEAN
)

CREATE INDEX IF NOT EXISTS idx_ledger_settings_company ON tab_ledger_settings(company_id);

-----------------------------------------------------------
-- TABLE: TabLocation
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_location (
    location_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    location_name VARCHAR(200) NOT NULL,
    branch_id INT,
    warehouse_id INT,
    address VARCHAR(300),
    city VARCHAR(100),
    state VARCHAR(100),
    country VARCHAR(100),

    CONSTRAINT fk_location_branch FOREIGN KEY (branch_id) REFERENCES tab_branch_master(branch_id),
    CONSTRAINT fk_location_warehouse FOREIGN KEY (warehouse_id) REFERENCES tab_warehouse_master(warehouse_id)
);

CREATE INDEX IF NOT EXISTS idx_location_company ON tab_location(company_id);

-----------------------------------------------------------
-- TABLE: TabPartyAddress
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_party_address (
    address_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_id INT NOT NULL,
    address_type_id INT NOT NULL,
    attention VARCHAR(200),
    country VARCHAR(100),
    address_line1 VARCHAR(200),
    address_line2 VARCHAR(200),
    city VARCHAR(100),
    state VARCHAR(100),
    pin_code VARCHAR(20),
    phone VARCHAR(20),
    fax VARCHAR(20),
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_on TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_party_address_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_party_address_type FOREIGN KEY (address_type_id) REFERENCES tab_address_type_master(address_type_id)
);

CREATE INDEX IF NOT EXISTS idx_party_address_company ON tab_party_address(company_id);
CREATE INDEX IF NOT EXISTS idx_party_address_party ON tab_party_address(party_id);

-----------------------------------------------------------
-- TABLE: TabPartyBankDetails
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_party_bank_details (
    bank_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_id INT NOT NULL,
    account_holder_name VARCHAR(200),
    bank_name VARCHAR(200),
    account_number VARCHAR(50),
    ifsc VARCHAR(20),
    branch VARCHAR(200),
    micr VARCHAR(20),
    is_default BOOLEAN DEFAULT FALSE,
    created_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_party_bank_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id)
);

CREATE INDEX IF NOT EXISTS idx_party_bank_company ON tab_party_bank_details(company_id);

-----------------------------------------------------------
-- TABLE: TabPartyContactPersons
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_party_contact_persons (
    contact_person_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_id INT NOT NULL,
    salutation VARCHAR(20),
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    email VARCHAR(200),
    work_phone VARCHAR(20),
    mobile VARCHAR(20),
    is_primary BOOLEAN DEFAULT FALSE,
    created_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_party_contact_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id)
);

CREATE INDEX IF NOT EXISTS idx_party_contact_company ON tab_party_contact_persons(company_id);

-----------------------------------------------------------
-- TABLE: TabPartyMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_party_master (
    party_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_type VARCHAR(20) NOT NULL,
    salutation VARCHAR(20),
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    company_name VARCHAR(200),
    display_name VARCHAR(200) NOT NULL,
    email VARCHAR(200),
    phone_work VARCHAR(20),
    phone_mobile VARCHAR(20),
    preferred_language VARCHAR(50),
    gst_treatment VARCHAR(100),
    place_of_supply VARCHAR(100),
    pan VARCHAR(20),
    msme_registered BOOLEAN,
    tax_preference VARCHAR(20),
    tds VARCHAR(50),
    currency_id INT,
    accounts_receivable_account INT,
    accounts_payable_account INT,
    opening_balance DECIMAL(18, 2),
    opening_balance_type VARCHAR(20),
    payment_terms VARCHAR(50),
    enable_portal BOOLEAN,
    website_url VARCHAR(200),
    department VARCHAR(100),
    remarks TEXT,
    created_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    party_type_id INT,
    gstno VARCHAR(40),

    CONSTRAINT fk_party_type FOREIGN KEY (party_type_id) REFERENCES tab_party_type_master(party_type_id)
);

CREATE INDEX IF NOT EXISTS idx_party_company ON tab_party_master(company_id);
CREATE INDEX IF NOT EXISTS idx_party_type ON tab_party_master(party_type);

-----------------------------------------------------------
-- TABLE: TabPartyReportingTags
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_party_reporting_tags (
    tag_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_id INT NOT NULL,
    tag_name VARCHAR(100),
    reporting_tag_id INT,
    created_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_party_tags_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id)
);

CREATE INDEX IF NOT EXISTS idx_party_tags_company ON tab_party_reporting_tags(company_id);

-----------------------------------------------------------
-- TABLE: TabPartyTypeMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_party_type_master (
    party_type_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_type_code VARCHAR(20) NOT NULL,
    party_type_name VARCHAR(100) NOT NULL,
    description VARCHAR(250),
    is_customer BOOLEAN NOT NULL DEFAULT FALSE,
    is_vendor BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    sort_order INT NOT NULL DEFAULT 1,
    created_on TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    party_id INT,
    party_type VARCHAR(20)
)

CREATE INDEX IF NOT EXISTS idx_party_type_master_company ON tab_party_type_master(company_id);

-----------------------------------------------------------
-- TABLE: TabPaymentModeMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_payment_mode_master (
    payment_mode_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    mode_name VARCHAR(100),
    is_default BOOLEAN
)

CREATE INDEX IF NOT EXISTS idx_payment_mode_company ON tab_payment_mode_master(company_id);

-----------------------------------------------------------
-- TABLE: TabPaymentTermMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_payment_term_master (
    term_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    term_name VARCHAR(100),
    number_of_days INT
)

CREATE INDEX IF NOT EXISTS idx_payment_term_company ON tab_payment_term_master(company_id);

-----------------------------------------------------------
-- TABLE: TabProductGroupMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_product_group_master (
    product_group_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    product_group_name VARCHAR(200) NOT NULL,
    short_name VARCHAR(50),
    alias_name VARCHAR(100),
    trading_account_id INT,

    CONSTRAINT fk_product_group_account FOREIGN KEY (trading_account_id) REFERENCES tab_account_master(account_id)
);

CREATE INDEX IF NOT EXISTS idx_product_group_company ON tab_product_group_master(company_id);

-----------------------------------------------------------
-- TABLE: TabProductMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_product_master (
    product_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    product_type VARCHAR(20),
    product_name VARCHAR(200),
    selling_price DECIMAL(18, 2),
    sales_account VARCHAR(100),
    sales_description VARCHAR(500),
    cost_price DECIMAL(18, 2),
    purchase_account VARCHAR(100),
    purchase_description VARCHAR(500),
    preferred_vendor_id INT,
    sellable BOOLEAN,
    purchasable BOOLEAN,
    unit_id INT,
    hsn_id INT,
    sac_id INT,
    tax_preference_id INT,
    tax_group_id INT,
    product_group_id INT,
    product_packing_unit INT,
    opening_quantity NUMERIC(18,4),
    opening_price DOUBLE PRECISION,
    opening_amount DOUBLE PRECISION,
    purchase_effected_account INT,
    sale_effected_account INT,
    selling_price2 DOUBLE PRECISION,
    selling_price3 DOUBLE PRECISION,
    selling_price4 DOUBLE PRECISION,
    selling_price5 DOUBLE PRECISION,
    min_quantity NUMERIC(18,4),
    max_quantity NUMERIC(18,4),
    reorder_quantity NUMERIC(18,4),
    tax_exemption_id INT,

    CONSTRAINT fk_product_hsn FOREIGN KEY (hsn_id) REFERENCES tab_hsn_master(hsn_id),
    CONSTRAINT fk_product_unit FOREIGN KEY (unit_id) REFERENCES tab_unit_master(unit_id),
    CONSTRAINT fk_product_packing_unit FOREIGN KEY (product_packing_unit) REFERENCES tab_unit_master(unit_id),
    CONSTRAINT fk_product_group FOREIGN KEY (product_group_id) REFERENCES tab_product_group_master(product_group_id),
    CONSTRAINT fk_product_purchase_account FOREIGN KEY (purchase_effected_account) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_product_sale_account FOREIGN KEY (sale_effected_account) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_product_tax_exemption FOREIGN KEY (tax_exemption_id) REFERENCES tab_tax_exemption_master(tax_exemption_id),
    CONSTRAINT fk_product_sac FOREIGN KEY (sac_id) REFERENCES tab_sac_master(sac_id),
    CONSTRAINT fk_product_tax_group FOREIGN KEY (tax_group_id) REFERENCES tab_tax_group_master(tax_group_id),
    CONSTRAINT fk_product_tax_preference FOREIGN KEY (tax_preference_id) REFERENCES tab_tax_preference_master(tax_preference_id)
);

CREATE INDEX IF NOT EXISTS idx_product_company ON tab_product_master(company_id);

-----------------------------------------------------------
-- TABLE: TabProjectMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_project_master (
    project_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    project_name VARCHAR(200) NOT NULL,
    start_date DATE,
    end_date DATE,
    status VARCHAR(50),
    description VARCHAR(500)
)

CREATE INDEX IF NOT EXISTS idx_project_company ON tab_project_master(company_id);

-----------------------------------------------------------
-- TABLE: TabPurchaseOrder
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_purchase_order (
    purchase_order_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    vendor_id INT NOT NULL,
    location_id INT NOT NULL,
    customer_id INT,
    purchase_order_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    date DATE NOT NULL,
    delivery_date DATE,
    shipment_preference VARCHAR(100),
    payment_terms VARCHAR(50) NOT NULL,
    reverse_charge BOOLEAN,
    customer_notes VARCHAR(500),
    terms TEXT,

    CONSTRAINT fk_purchase_order_vendor FOREIGN KEY (vendor_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_purchase_order_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id),
    CONSTRAINT fk_purchase_order_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id)
);

CREATE INDEX IF NOT EXISTS idx_purchase_order_company ON tab_purchase_order(company_id);

-----------------------------------------------------------
-- TABLE: TabPurchaseOrderItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_purchase_order_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    purchase_order_id INT,
    item_details VARCHAR(200),
    account_id INT,
    quantity DECIMAL(18,4) NOT NULL,
    rate DECIMAL(18, 2) NOT NULL,
    tax_id INT,
    discount_pct DECIMAL(5, 2),
    amount DECIMAL(18, 2) NOT NULL,
    tds_tcs VARCHAR(5),
    adjustment DECIMAL(18, 2),

    CONSTRAINT fk_purchase_order_items_order FOREIGN KEY (purchase_order_id) REFERENCES tab_purchase_order(purchase_order_id),
    CONSTRAINT fk_purchase_order_items_account FOREIGN KEY (account_id) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_purchase_order_items_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id)
);

CREATE INDEX IF NOT EXISTS idx_purchase_order_items_company ON tab_purchase_order_items(company_id);

-----------------------------------------------------------
-- TABLE: TabQuote
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_quote (
    quote_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_id INT NOT NULL,
    location_id INT,
    quote_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    quote_date DATE NOT NULL,
    expiry_date DATE,
    salesperson_id INT,
    subject VARCHAR(300),
    customer_notes VARCHAR(500),
    terms TEXT,
    auto_create_retainer BOOLEAN,
    percentage_to_be_collected DECIMAL(8, 2),

    CONSTRAINT fk_quote_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_quote_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id)
);

CREATE INDEX IF NOT EXISTS idx_quote_company ON tab_quote(company_id);

-----------------------------------------------------------
-- TABLE: TabQuoteItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_quote_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    quote_id INT,
    item_details VARCHAR(200),
    quantity DECIMAL(18,4),
    rate DECIMAL(18, 2),
    tax_id INT,
    amount DECIMAL(18, 2),
    discount_pct DECIMAL(5, 2),
    tds_tcs VARCHAR(5),
    adjustment DECIMAL(18, 2),

    CONSTRAINT fk_quote_items_quote FOREIGN KEY (quote_id) REFERENCES tab_quote(quote_id),
    CONSTRAINT fk_quote_items_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id)
);

CREATE INDEX IF NOT EXISTS idx_quote_items_company ON tab_quote_items(company_id);

-----------------------------------------------------------
-- TABLE: TabRecordExpense
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_record_expense (
    expense_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    location_id INT NOT NULL,
    expense_date DATE NOT NULL,
    expense_account_id INT NOT NULL,
    amount DECIMAL(18, 2) NOT NULL,
    paid_through_account_id INT NOT NULL,
    expense_type VARCHAR(20) NOT NULL,
    sac VARCHAR(20),
    vendor_id INT,
    gst_treatment VARCHAR(50) NOT NULL,
    source_of_supply VARCHAR(50) NOT NULL,
    destination_of_supply VARCHAR(50) NOT NULL,
    reverse_charge BOOLEAN,
    tax_id INT,
    amount_is VARCHAR(20) NOT NULL,
    invoice_number VARCHAR(50),
    notes VARCHAR(500),
    customer_id INT,

    CONSTRAINT fk_record_expense_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id),
    CONSTRAINT fk_record_expense_account FOREIGN KEY (expense_account_id) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_record_expense_paid_through FOREIGN KEY (paid_through_account_id) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_record_expense_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id),
    CONSTRAINT fk_record_expense_vendor FOREIGN KEY (vendor_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_record_expense_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id)
);

CREATE INDEX IF NOT EXISTS idx_record_expense_company ON tab_record_expense(company_id);

-----------------------------------------------------------
-- TABLE: TabRecordMileage
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_record_mileage (
    mileage_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    location_id INT NOT NULL,
    mileage_date DATE NOT NULL,
    employee_id INT,
    calculation_method VARCHAR(20) NOT NULL,
    distance_km DECIMAL(10, 2),
    odometer_start DECIMAL(10, 2),
    odometer_end DECIMAL(10, 2),
    amount DECIMAL(18, 2) NOT NULL,
    paid_through_account_id INT NOT NULL,
    vendor_id INT,
    invoice_number VARCHAR(50),
    notes VARCHAR(500),
    customer_id INT,

    CONSTRAINT fk_record_mileage_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id),
    CONSTRAINT fk_record_mileage_employee FOREIGN KEY (employee_id) REFERENCES tab_employee(employee_id),
    CONSTRAINT fk_record_mileage_account FOREIGN KEY (paid_through_account_id) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_record_mileage_vendor FOREIGN KEY (vendor_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_record_mileage_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id)
);

CREATE INDEX IF NOT EXISTS idx_record_mileage_company ON tab_record_mileage(company_id);

-----------------------------------------------------------
-- TABLE: TabRecurringBill
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_recurring_bill (
    recurring_bill_id INT,
    company_id INT NOT NULL,
    vendor_id INT,
    location VARCHAR(100),
    profile_name VARCHAR(200),
    repeat_every VARCHAR(50),
    start_on DATE,
    ends_on DATE,
    never_expires BOOLEAN,
    accounts_payable VARCHAR(200),
    payment_terms VARCHAR(200),
    reverse_charge_applicable BOOLEAN,
    warehouse_location VARCHAR(200),
    transaction_level VARCHAR(100),
    sub_total DECIMAL(18, 2),
    discount DECIMAL(18, 2),
    tds VARCHAR(100),
    adjustment DECIMAL(18, 2),
    total_amount DECIMAL(18, 2),
    notes TEXT
)

CREATE INDEX IF NOT EXISTS idx_recurring_bill_company ON tab_recurring_bill(company_id);

-----------------------------------------------------------
-- TABLE: TabRecurringBillItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_recurring_bill_items (
    item_id INT,
    company_id INT NOT NULL,
    recurring_bill_id INT,
    item_details VARCHAR(300),
    account VARCHAR(200),
    quantity DECIMAL(18,4),
    rate DECIMAL(18, 2),
    tax VARCHAR(100),
    customer_details VARCHAR(200),
    amount DECIMAL(18, 2)
)

CREATE INDEX IF NOT EXISTS idx_recurring_bill_items_company ON tab_recurring_bill_items(company_id);

-----------------------------------------------------------
-- TABLE: TabRecurringExpense
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_recurring_expense (
    recurring_expense_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    profile_name VARCHAR(150) NOT NULL,
    location_id INT NOT NULL,
    repeat_every VARCHAR(20) NOT NULL,
    start_date DATE NOT NULL,
    ends_on DATE,
    never_expires BOOLEAN,
    expense_account_id INT NOT NULL,
    amount DECIMAL(18, 2) NOT NULL,
    paid_through_account_id INT NOT NULL,
    expense_type VARCHAR(20) NOT NULL,
    sac VARCHAR(20),
    vendor_id INT,
    gst_treatment VARCHAR(50) NOT NULL,
    source_of_supply VARCHAR(50) NOT NULL,
    destination_of_supply VARCHAR(50) NOT NULL,
    reverse_charge BOOLEAN,
    tax_id INT,
    amount_is VARCHAR(20) NOT NULL,
    notes VARCHAR(500),
    customer_id INT,

    CONSTRAINT fk_recurring_expense_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id),
    CONSTRAINT fk_recurring_expense_account FOREIGN KEY (expense_account_id) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_recurring_expense_paid_through FOREIGN KEY (paid_through_account_id) REFERENCES tab_account_master(account_id),
    CONSTRAINT fk_recurring_expense_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id),
    CONSTRAINT fk_recurring_expense_vendor FOREIGN KEY (vendor_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_recurring_expense_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id)
);

CREATE INDEX IF NOT EXISTS idx_recurring_expense_company ON tab_recurring_expense(company_id);

-----------------------------------------------------------
-- TABLE: TabRecurringInvoice
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_recurring_invoice (
    recurring_invoice_id INT,
    company_id INT NOT NULL,
    customer_id INT,
    location VARCHAR(100),
    profile_name VARCHAR(150),
    order_number VARCHAR(50),
    repeat_every VARCHAR(20),
    start_on DATE,
    ends_on DATE,
    payment_terms VARCHAR(50),
    accounts_receivable INT,
    salesperson_id INT,
    subject VARCHAR(250),
    warehouse_location VARCHAR(250),
    customer_notes TEXT,
    terms_conditions TEXT,
    round_off DECIMAL(10, 2)
)

CREATE INDEX IF NOT EXISTS idx_recurring_invoice_company ON tab_recurring_invoice(company_id);

-----------------------------------------------------------
-- TABLE: TabRecurringInvoiceItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_recurring_invoice_items (
    item_id INT,
    company_id INT NOT NULL,
    recurring_invoice_id INT,
    item_details VARCHAR(200),
    quantity DECIMAL(18,4),
    rate DECIMAL(10, 2),
    tax_id INT,
    customer_details_id INT,
    amount DECIMAL(10, 2),
    discount_pct DECIMAL(5, 2),
    tds_tcs VARCHAR(5),
    adjustment DECIMAL(18, 2)
)

CREATE INDEX IF NOT EXISTS idx_recurring_invoice_items_company ON tab_recurring_invoice_items(company_id);

-----------------------------------------------------------
-- TABLE: TabRetainerInvoice
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_retainer_invoice (
    retainer_invoice_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_id INT NOT NULL,
    location_id INT,
    retainer_invoice_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    invoice_date DATE NOT NULL,
    project_id INT,
    customer_notes VARCHAR(500),
    terms TEXT,

    CONSTRAINT fk_retainer_invoice_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_retainer_invoice_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id),
    CONSTRAINT fk_retainer_invoice_project FOREIGN KEY (project_id) REFERENCES tab_project_master(project_id)
);

CREATE INDEX IF NOT EXISTS idx_retainer_invoice_company ON tab_retainer_invoice(company_id);

-----------------------------------------------------------
-- TABLE: TabRetainerInvoiceItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_retainer_invoice_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    retainer_invoice_id INT,
    description VARCHAR(200),
    amount DECIMAL(18, 2),

    CONSTRAINT fk_retainer_invoice_items_invoice FOREIGN KEY (retainer_invoice_id) REFERENCES tab_retainer_invoice(retainer_invoice_id)
);

CREATE INDEX IF NOT EXISTS idx_retainer_invoice_items_company ON tab_retainer_invoice_items(company_id);

-----------------------------------------------------------
-- TABLE: TabRoleMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_role_master (
    role_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    role_name VARCHAR(100),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
)

CREATE INDEX IF NOT EXISTS idx_role_company ON tab_role_master(company_id);

-----------------------------------------------------------
-- TABLE: TabRolePermission
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_role_permission (
    permission_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    role_id INT,
    tab_id INT NOT NULL,
    can_view BOOLEAN,
    can_add BOOLEAN,
    can_edit BOOLEAN,
    can_delete BOOLEAN,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_role_permission_role FOREIGN KEY (role_id) REFERENCES tab_role_master(role_id),
    CONSTRAINT fk_role_permission_tab FOREIGN KEY (tab_id) REFERENCES tabs(tab_id)
);

CREATE INDEX IF NOT EXISTS idx_role_permission_company ON tab_role_permission(company_id);

-----------------------------------------------------------
-- TABLE: Tabs
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tabs (
    tab_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tab_name VARCHAR(50) NOT NULL,
    tab_url VARCHAR(255),
    parent_id INT,
    icon_path VARCHAR(255),
    sort_order INT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_tabs_parent FOREIGN KEY (parent_id) REFERENCES tabs(tab_id)
);

CREATE INDEX IF NOT EXISTS idx_tabs_company ON tabs(company_id);

-----------------------------------------------------------
-- TABLE: TabSACMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_sac_master (
    sac_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    sac_code VARCHAR(20),
    description VARCHAR(300),
    tax_rate DECIMAL(5, 2)
)

CREATE INDEX IF NOT EXISTS idx_sac_company ON tab_sac_master(company_id);

-----------------------------------------------------------
-- TABLE: TabSalesOrder
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_sales_order (
    sales_order_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_id INT NOT NULL,
    location_id INT,
    sales_order_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    sales_order_date DATE NOT NULL,
    expected_shipment_date DATE,
    payment_terms VARCHAR(50) NOT NULL,
    salesperson_id INT,
    delivery_method VARCHAR(100),
    customer_notes VARCHAR(500),
    terms TEXT,
    warehouse_location VARCHAR(250),

    CONSTRAINT fk_sales_order_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id),
    CONSTRAINT fk_sales_order_location FOREIGN KEY (location_id) REFERENCES tab_location(location_id)
);

CREATE INDEX IF NOT EXISTS idx_sales_order_company ON tab_sales_order(company_id);

-----------------------------------------------------------
-- TABLE: TabSalesOrderItems
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_sales_order_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    sales_order_id INT,
    item_details VARCHAR(200),
    quantity DECIMAL(18,4) NOT NULL,
    rate DECIMAL(18, 2) NOT NULL,
    tax_id INT,
    amount DECIMAL(18, 2) NOT NULL,
    discount_pct DECIMAL(5, 2),
    tds_tcs VARCHAR(5),
    adjustment DECIMAL(18, 2),

    CONSTRAINT fk_sales_order_items_order FOREIGN KEY (sales_order_id) REFERENCES tab_sales_order(sales_order_id),
    CONSTRAINT fk_sales_order_items_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id)
);

CREATE INDEX IF NOT EXISTS idx_sales_order_items_company ON tab_sales_order_items(company_id);

-----------------------------------------------------------
-- TABLE: TabSalespersonMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_salesperson_master (
    salesperson_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    salesperson_name VARCHAR(200),
    email VARCHAR(200)
)

CREATE INDEX IF NOT EXISTS idx_salesperson_company ON tab_salesperson_master(company_id);

-----------------------------------------------------------
-- TABLE: TabTaxExemptionMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_tax_exemption_master (
    tax_exemption_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    exemption_reason VARCHAR(200) NOT NULL,
    description VARCHAR(500),
    exemption_type VARCHAR(20) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_on TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
)

CREATE INDEX IF NOT EXISTS idx_tax_exemption_company ON tab_tax_exemption_master(company_id);

-----------------------------------------------------------
-- TABLE: TabTaxGroupDetail
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_tax_group_detail (
    tax_group_detail_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tax_group_id INT NOT NULL,
    tax_id INT NOT NULL,
    apply_order INT NOT NULL,

    CONSTRAINT fk_tax_group_detail_group FOREIGN KEY (tax_group_id) REFERENCES tab_tax_group_master(tax_group_id),
    CONSTRAINT fk_tax_group_detail_tax FOREIGN KEY (tax_id) REFERENCES tab_tax_master(tax_id),
    CONSTRAINT ux_tax_group_tax UNIQUE (tax_group_id, tax_id)
);

CREATE INDEX IF NOT EXISTS idx_tax_group_detail_company ON tab_tax_group_detail(company_id);

-----------------------------------------------------------
-- TABLE: TabTaxGroupMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_tax_group_master (
    tax_group_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    group_name VARCHAR(200) NOT NULL,
    description VARCHAR(300),
    supply_type VARCHAR(20),
    cgst_per DOUBLE PRECISION,
    sgst_per DOUBLE PRECISION,
    igst_per DOUBLE PRECISION
)

CREATE INDEX IF NOT EXISTS idx_tax_group_company ON tab_tax_group_master(company_id);

-----------------------------------------------------------
-- TABLE: TabTaxMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_tax_master (
    tax_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tax_name VARCHAR(200),
    rate_percent DECIMAL(5, 2),
    tax_type VARCHAR(50)
)

CREATE INDEX IF NOT EXISTS idx_tax_company ON tab_tax_master(company_id);

-----------------------------------------------------------
-- TABLE: TabTaxPreferenceMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_tax_preference_master (
    tax_preference_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    preference_code VARCHAR(30) NOT NULL,
    preference_name VARCHAR(100) NOT NULL,
    is_gst_applicable BOOLEAN NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT ux_tax_preference_code UNIQUE (preference_code)
)

CREATE INDEX IF NOT EXISTS idx_tax_preference_company ON tab_tax_preference_master(company_id);

-----------------------------------------------------------
-- TABLE: TabTCSMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_tcs_master (
    tcs_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tax_name VARCHAR(100) NOT NULL,
    rate DECIMAL(5, 2) NOT NULL,
    nature_of_collection VARCHAR(100) NOT NULL,
    is_higher_rate BOOLEAN NOT NULL DEFAULT FALSE,
    start_date DATE,
    end_date DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
)

CREATE INDEX IF NOT EXISTS idx_tcs_company ON tab_tcs_master(company_id);

-----------------------------------------------------------
-- TABLE: TabTDSMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_tds_master (
    tds_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tax_name VARCHAR(100) NOT NULL,
    rate DECIMAL(5, 2) NOT NULL,
    section VARCHAR(50) NOT NULL,
    is_higher_rate BOOLEAN NOT NULL DEFAULT FALSE,
    start_date DATE,
    end_date DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
)

CREATE INDEX IF NOT EXISTS idx_tds_company ON tab_tds_master(company_id);

-----------------------------------------------------------
-- TABLE: TabTransporterMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_transporter_master (
    transporter_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    transporter_name VARCHAR(200),
    gst_number VARCHAR(20),
    contact_person VARCHAR(100),
    phone VARCHAR(20),
    address VARCHAR(300)
)

CREATE INDEX IF NOT EXISTS idx_transporter_company ON tab_transporter_master(company_id);

-----------------------------------------------------------
-- TABLE: TabUnitMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_unit_master (
    unit_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    unit_name VARCHAR(20),
    uqc VARCHAR(20),
    crt BIGINT,
    packing_units BIGINT
)

CREATE INDEX IF NOT EXISTS idx_unit_company ON tab_unit_master(company_id);

-----------------------------------------------------------
-- TABLE: TabUserMaster
-- Note: CompanyId already exists but as VARCHAR(50), we'll add INT version
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_user_master (
    user_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    username VARCHAR(100),
    password_hash VARCHAR(256),
    full_name VARCHAR(200),
    email VARCHAR(200),
    phone VARCHAR(20),
    is_active BOOLEAN,
    company_id_old VARCHAR(50), -- Keep old column for migration

);

CREATE INDEX IF NOT EXISTS idx_user_company ON tab_user_master(company_id);
CREATE INDEX IF NOT EXISTS idx_user_email ON tab_user_master(email);

-----------------------------------------------------------
-- TABLE: TabUserRole
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_user_role (
    company_id INT NOT NULL,
    user_id INT,
    role_id INT,

    CONSTRAINT fk_user_role_user FOREIGN KEY (user_id) REFERENCES tab_user_master(user_id),
    CONSTRAINT fk_user_role_role FOREIGN KEY (role_id) REFERENCES tab_role_master(role_id)
);

CREATE INDEX IF NOT EXISTS idx_user_role_company ON tab_user_role(company_id);

-----------------------------------------------------------
-- TABLE: TabVendorAdvanced
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_vendor_advanced (
    vendor_advance_id INT,
    company_id INT NOT NULL,
    vendor_name VARCHAR(200),
    gst_treatment VARCHAR(100),
    source_of_supply VARCHAR(50),
    destination_of_supply VARCHAR(50),
    location VARCHAR(100),
    payment_number VARCHAR(50),
    description_of_supply TEXT,
    payment_made DECIMAL(18, 2),
    reverse_charge BOOLEAN,
    tds VARCHAR(50),
    payment_date DATE,
    payment_mode VARCHAR(100),
    paid_through VARCHAR(200),
    deposit_to VARCHAR(200),
    reference_number VARCHAR(200),
    notes TEXT
)

CREATE INDEX IF NOT EXISTS idx_vendor_advanced_company ON tab_vendor_advanced(company_id);

-----------------------------------------------------------
-- TABLE: TabVendorCredit
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_vendor_credit (
    vendor_credit_id INT,
    company_id INT NOT NULL,
    vendor_name VARCHAR(200),
    location VARCHAR(100),
    credit_note_number VARCHAR(50),
    order_number VARCHAR(100),
    vendor_credit_date DATE,
    subject VARCHAR(300),
    accounts_payable VARCHAR(100),
    reverse_charge BOOLEAN,
    notes TEXT
)

CREATE INDEX IF NOT EXISTS idx_vendor_credit_company ON tab_vendor_credit(company_id);

-----------------------------------------------------------
-- TABLE: TabWarehouseMaster
-----------------------------------------------------------
CREATE TABLE IF NOT EXISTS tab_warehouse_master (
    warehouse_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    warehouse_name VARCHAR(150) NOT NULL,
    branch_id INT,
    address VARCHAR(300),

    CONSTRAINT fk_warehouse_branch FOREIGN KEY (branch_id) REFERENCES tab_branch_master(branch_id)
);

CREATE INDEX IF NOT EXISTS idx_warehouse_company ON tab_warehouse_master(company_id);

-----------------------------------------------------------
-- VIEWS (Updated with CompanyId filtering)
-----------------------------------------------------------

CREATE OR REPLACE VIEW account AS
SELECT account_id, account_name, account_type, company_id
FROM tab_account_master;

CREATE OR REPLACE VIEW accounts_payable AS
SELECT account_id AS accounts_payable_id, account_name, company_id
FROM tab_account_master 
WHERE account_type = 'Liability';

CREATE OR REPLACE VIEW accounts_receivable_ref AS
SELECT account_id AS accounts_receivable, account_name, company_id
FROM tab_account_master 
WHERE account_type = 'Asset';

CREATE OR REPLACE VIEW customer AS
SELECT party_id AS customer_id, *, company_id
FROM tab_party_master 
WHERE party_type = 'Customer';

CREATE OR REPLACE VIEW expense_account AS
SELECT account_id AS expense_account_id, account_name, company_id
FROM tab_account_master 
WHERE account_type = 'Expense';

CREATE OR REPLACE VIEW hsn_code_ref AS
SELECT hsn_code, company_id
FROM tab_hsn_master;

CREATE OR REPLACE VIEW igst_ref AS
SELECT tax_id AS igst, company_id
FROM tab_tax_master 
WHERE tax_type = 'IGST';

CREATE OR REPLACE VIEW paid_through_account AS
SELECT account_id AS paid_through_account_id, account_name, company_id
FROM tab_account_master 
WHERE account_type IN ('Cash', 'Bank');

CREATE OR REPLACE VIEW sac_ref AS
SELECT sac_code AS sac, company_id
FROM tab_sac_master;

CREATE OR REPLACE VIEW vendor AS
SELECT party_id AS vendor_id, *, company_id
FROM tab_party_master 
WHERE party_type = 'Vendor';

-----------------------------------------------------------
-- END OF SCHEMA CREATION
-- Note: Stored Procedures will be converted to PostgreSQL functions
-- in a separate migration script due to complexity
-----------------------------------------------------------

