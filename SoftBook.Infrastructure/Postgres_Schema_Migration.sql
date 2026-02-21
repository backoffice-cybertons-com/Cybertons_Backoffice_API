-----------------------------------------------------------
-- POSTGRESQL REFINED SCHEMA MIGRATION (REF: SoftAccDB_SQL.sql)
-- DATABASE: SoftAccDb (Shared Tenant Database)
-- CONVENTION: snake_case for all identifiers
-- MULTI-TENANCY: company_id INT NOT NULL in every table
-----------------------------------------------------------

-- CLEANUP (Optional/CAUTION: Drops existing tables to ensure clean state)
-- DROP SCHEMA public CASCADE;
-- CREATE SCHEMA public;

-----------------------------------------------------------
-- 1. CORE MASTERS
-----------------------------------------------------------

CREATE TABLE IF NOT EXISTS tab_account_master (
    account_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    account_type VARCHAR(100),
    account_name VARCHAR(200),
    is_sub_account BOOLEAN DEFAULT FALSE,
    parent_account_id INT,
    account_code VARCHAR(50),
    description TEXT
);

CREATE TABLE IF NOT EXISTS tab_address_type_master (
    address_type_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    address_type_code VARCHAR(30) NOT NULL,
    address_type_name VARCHAR(100) NOT NULL,
    usage_category VARCHAR(30) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    sort_order INT DEFAULT 1,
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tab_bank_master (
    bank_master_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    bank_name VARCHAR(200) NOT NULL,
    branch VARCHAR(150),
    account_number VARCHAR(50),
    ifsc VARCHAR(20),
    micr VARCHAR(20)
);

CREATE TABLE IF NOT EXISTS tab_branch_master (
    branch_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    branch_name VARCHAR(150) NOT NULL,
    address TEXT,
    city VARCHAR(100),
    state VARCHAR(100),
    country VARCHAR(100),
    phone VARCHAR(20),
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS tab_cost_center_master (
    cost_center_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    cost_center_name VARCHAR(150) NOT NULL,
    description TEXT
);

CREATE TABLE IF NOT EXISTS tab_currency_master (
    currency_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    currency_code VARCHAR(10),
    currency_symbol VARCHAR(10),
    currency_name VARCHAR(100),
    decimal_places INT DEFAULT 2,
    format VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS tab_currency_adjustment (
    adjustment_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    currency_id INT NOT NULL,
    adjustment_date DATE NOT NULL,
    exchange_rate NUMERIC(18,6) NOT NULL,
    to_currency VARCHAR(10) NOT NULL,
    notes TEXT,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tab_employee (
    employee_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    employee_name VARCHAR(200),
    email VARCHAR(200),
    phone VARCHAR(20),
    department VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS tab_fiscal_year_master (
    fiscal_year_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    is_current BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS tab_godown_master (
    godown_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    warehouse_id INT,
    godown_name VARCHAR(150)
);

CREATE TABLE IF NOT EXISTS tab_hsn_master (
    hsn_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    hsn_code VARCHAR(20),
    description TEXT,
    tax_rate NUMERIC(5,2)
);

CREATE TABLE IF NOT EXISTS tab_sac_master (
    sac_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    sac_code VARCHAR(20),
    description TEXT,
    tax_rate NUMERIC(5,2)
);

CREATE TABLE IF NOT EXISTS tab_unit_master (
    unit_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    unit_name VARCHAR(20),
    uqc VARCHAR(20),
    crt BIGINT,
    packing_units BIGINT
);

CREATE TABLE IF NOT EXISTS tab_warehouse_master (
    warehouse_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    warehouse_name VARCHAR(150) NOT NULL,
    branch_id INT,
    address TEXT
);

CREATE TABLE IF NOT EXISTS tab_location (
    location_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    location_name VARCHAR(200) NOT NULL,
    branch_id INT,
    warehouse_id INT,
    address TEXT,
    city VARCHAR(100),
    state VARCHAR(100),
    country VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS tab_ledger_settings (
    setting_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    auto_numbering BOOLEAN DEFAULT TRUE,
    default_currency VARCHAR(10),
    enable_tcs BOOLEAN DEFAULT FALSE,
    enable_tds BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS tab_salesperson_master (
    salesperson_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    salesperson_name VARCHAR(200),
    email VARCHAR(200)
);

CREATE TABLE IF NOT EXISTS tab_project_master (
    project_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    project_name VARCHAR(200) NOT NULL,
    start_date DATE,
    end_date DATE,
    status VARCHAR(50),
    description TEXT
);

CREATE TABLE IF NOT EXISTS tab_payment_mode_master (
    payment_mode_id_internal SERIAL PRIMARY KEY, -- Fixed PK name for universal function
    company_id INT NOT NULL,
    mode_name VARCHAR(100),
    is_default BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS tab_payment_term_master (
    term_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    term_name VARCHAR(100),
    number_of_days INT
);

-----------------------------------------------------------
-- 2. PARTY & CRM
-----------------------------------------------------------

CREATE TABLE IF NOT EXISTS tab_party_type_master (
    party_type_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_type_code VARCHAR(20) NOT NULL,
    party_type_name VARCHAR(100) NOT NULL,
    description TEXT,
    is_customer BOOLEAN DEFAULT FALSE,
    is_vendor BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    sort_order INT DEFAULT 1,
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    party_id INT, -- Logical back-link if needed
    party_type VARCHAR(20)
);

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
    gst_no VARCHAR(40),
    msme_registered BOOLEAN DEFAULT FALSE,
    tax_preference VARCHAR(20),
    tds VARCHAR(50),
    currency_id INT,
    accounts_receivable_account INT,
    accounts_payable_account INT,
    opening_balance NUMERIC(18,2),
    opening_balance_type VARCHAR(20),
    payment_terms VARCHAR(50),
    enable_portal BOOLEAN DEFAULT FALSE,
    website_url VARCHAR(200),
    department VARCHAR(100),
    remarks TEXT,
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    party_type_id INT
);

CREATE TABLE IF NOT EXISTS tab_party_address (
    address_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_id INT NOT NULL,
    address_type_id INT NOT NULL,
    attention VARCHAR(200),
    country VARCHAR(100),
    address_line1 TEXT,
    address_line2 TEXT,
    city VARCHAR(100),
    state VARCHAR(100),
    pin_code VARCHAR(20),
    phone VARCHAR(20),
    fax VARCHAR(20),
    is_default BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

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
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

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
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tab_party_reporting_tags (
    tag_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    party_id INT NOT NULL,
    tag_name VARCHAR(100),
    reporting_tag_id INT,
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-----------------------------------------------------------
-- 3. PRODUCT & INVENTORY
-----------------------------------------------------------

CREATE TABLE IF NOT EXISTS tab_product_group_master (
    product_group_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    product_group_name VARCHAR(200) NOT NULL,
    short_name VARCHAR(50),
    alias_name VARCHAR(100),
    trading_account_id INT
);

CREATE TABLE IF NOT EXISTS tab_product_master (
    product_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    product_type VARCHAR(20),
    product_name VARCHAR(200),
    selling_price NUMERIC(18,2),
    sales_account VARCHAR(100),
    sales_description TEXT,
    cost_price NUMERIC(18,2),
    purchase_account VARCHAR(100),
    purchase_description TEXT,
    preferred_vendor_id INT,
    sellable BOOLEAN DEFAULT TRUE,
    purchasable BOOLEAN DEFAULT TRUE,
    unit_id INT,
    hsn_id INT,
    sac_id INT,
    tax_preference_id INT,
    tax_group_id INT,
    product_group_id INT,
    product_packing_unit INT,
    opening_quantity NUMERIC(18,2), -- float in source, numeric better
    opening_price NUMERIC(18,2),
    opening_amount NUMERIC(18,2),
    purchase_effected_account INT,
    sale_effected_account INT,
    selling_price_2 NUMERIC(18,2),
    selling_price_3 NUMERIC(18,2),
    selling_price_4 NUMERIC(18,2),
    selling_price_5 NUMERIC(18,2),
    min_quantity NUMERIC(18,2),
    max_quantity NUMERIC(18,2),
    re_order_quantity NUMERIC(18,2),
    tax_exemption_id INT
);


-----------------------------------------------------------
-- 4. TRANSACTIONS - SALES
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
    subject TEXT,
    customer_notes TEXT,
    terms TEXT,
    auto_create_retainer BOOLEAN DEFAULT FALSE,
    percentage_to_be_collected NUMERIC(8,2)
);

CREATE TABLE IF NOT EXISTS tab_quote_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    quote_id INT,
    item_details TEXT,
    quantity NUMERIC(18,2),
    rate NUMERIC(18,2),
    tax_id INT,
    amount NUMERIC(18,2),
    discount_pct NUMERIC(5,2),
    tds_tcs VARCHAR(5),
    adjustment NUMERIC(18,2)
);

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
    customer_notes TEXT,
    terms TEXT,
    warehouse_location VARCHAR(250)
);

CREATE TABLE IF NOT EXISTS tab_sales_order_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    sales_order_id INT,
    item_details TEXT,
    quantity NUMERIC(18,2) NOT NULL,
    rate NUMERIC(18,2) NOT NULL,
    tax_id INT,
    amount NUMERIC(18,2) NOT NULL,
    discount_pct NUMERIC(5,2),
    tds_tcs VARCHAR(5),
    adjustment NUMERIC(18,2)
);

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
    subject TEXT,
    customer_notes TEXT,
    terms TEXT,
    warehouse_location VARCHAR(250)
);

CREATE TABLE IF NOT EXISTS tab_invoice_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    invoice_id INT,
    item_details TEXT,
    quantity NUMERIC(18,2) NOT NULL,
    rate NUMERIC(18,2) NOT NULL,
    tax_id INT,
    amount NUMERIC(18,2) NOT NULL,
    discount_pct NUMERIC(5,2),
    tds_tcs VARCHAR(5),
    adjustment NUMERIC(18,2)
);

CREATE TABLE IF NOT EXISTS tab_invoice_payment (
    invoice_payment_id_internal SERIAL PRIMARY KEY, -- Source had no PK, adding one
    company_id INT NOT NULL,
    invoice_payment_id INT, -- Logical link from source
    customer_name VARCHAR(200),
    location VARCHAR(100),
    amount_received NUMERIC(18,2),
    bank_charges NUMERIC(18,2),
    payment_date DATE,
    payment_number VARCHAR(50),
    payment_mode VARCHAR(100),
    deposit_to VARCHAR(200),
    reference_number VARCHAR(200),
    tax_deducted BOOLEAN DEFAULT FALSE,
    tds_tax_amount_id INT,
    notes TEXT,
    reference_no VARCHAR(50),
    notes_text TEXT
);

CREATE TABLE IF NOT EXISTS tab_customer_advance (
    customer_advance_id_internal SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_advance_id INT,
    customer_name VARCHAR(200),
    place_of_supply VARCHAR(100),
    location VARCHAR(100),
    description_of_supply TEXT,
    amount_received NUMERIC(18,2),
    bank_charges NUMERIC(18,2),
    tax VARCHAR(100),
    payment_date DATE,
    payment_number VARCHAR(50),
    payment_mode VARCHAR(100),
    deposit_to VARCHAR(200),
    reference_number VARCHAR(200),
    notes TEXT
);

CREATE TABLE IF NOT EXISTS tab_retainer_invoice (
    retainer_invoice_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_id INT NOT NULL,
    location_id INT,
    retainer_invoice_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    invoice_date DATE NOT NULL,
    project_id INT,
    customer_notes TEXT,
    terms TEXT
);

CREATE TABLE IF NOT EXISTS tab_retainer_invoice_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    retainer_invoice_id INT,
    description TEXT,
    amount NUMERIC(18,2)
);

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
    notes TEXT,
    terms TEXT,
    subject TEXT
);

CREATE TABLE IF NOT EXISTS tab_credit_notes_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    credit_note_id INT,
    item_details TEXT,
    account_id INT,
    quantity NUMERIC(18,2),
    rate NUMERIC(18,2),
    tax_id INT,
    amount NUMERIC(18,2),
    discount_pct NUMERIC(5,2),
    tds_tcs VARCHAR(5)
);

CREATE TABLE IF NOT EXISTS tab_delivery_challan (
    delivery_challan_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    customer_id INT NOT NULL,
    location_id INT,
    challan_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    challan_date DATE NOT NULL,
    challan_type VARCHAR(50) NOT NULL,
    customer_notes TEXT,
    terms TEXT,
    warehouse_location VARCHAR(250)
);

CREATE TABLE IF NOT EXISTS tab_delivery_challan_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    delivery_challan_id INT,
    item_details TEXT,
    quantity NUMERIC(18,2) NOT NULL,
    rate NUMERIC(18,2) NOT NULL,
    tax_id INT,
    amount NUMERIC(18,2) NOT NULL,
    discount_pct NUMERIC(5,2),
    adjustment NUMERIC(18,2)
);

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
    transporter_doc_date DATE
);

CREATE TABLE IF NOT EXISTS tab_eway_bills_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    eway_bill_id INT,
    item_details VARCHAR(200),
    hsn_code INT,
    quantity NUMERIC(18,2),
    taxable_amount NUMERIC(18,2),
    igst INT,
    cess NUMERIC(18,2)
);

CREATE TABLE IF NOT EXISTS tab_recurring_invoice (
    recurring_invoice_id_internal SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    recurring_invoice_id INT, -- logical ID from source
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
    round_off NUMERIC(10,2)
);

CREATE TABLE IF NOT EXISTS tab_recurring_invoice_items (
    item_id_internal SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    item_id INT,
    recurring_invoice_id INT,
    item_details VARCHAR(200),
    quantity NUMERIC(10,2),
    rate NUMERIC(10,2),
    tax_id INT,
    customer_details_id INT,
    amount NUMERIC(10,2),
    discount_pct NUMERIC(5,2),
    tds_tcs VARCHAR(5),
    adjustment NUMERIC(18,2)
);


-----------------------------------------------------------
-- 5. TRANSACTIONS - PURCHASE
-----------------------------------------------------------

CREATE TABLE IF NOT EXISTS tab_purchase_order (
    purchase_order_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    vendor_id INT NOT NULL,
    location_id INT NOT NULL,
    customer_id INT,
    purchase_order_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    order_date DATE NOT NULL, --  Date in source
    delivery_date DATE,
    shipment_preference VARCHAR(100),
    payment_terms VARCHAR(50) NOT NULL,
    reverse_charge BOOLEAN DEFAULT FALSE,
    customer_notes TEXT,
    terms TEXT
);

CREATE TABLE IF NOT EXISTS tab_purchase_order_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    purchase_order_id INT,
    item_details TEXT,
    account_id INT,
    quantity NUMERIC(18,2) NOT NULL,
    rate NUMERIC(18,2) NOT NULL,
    tax_id INT,
    discount_pct NUMERIC(5,2),
    amount NUMERIC(18,2) NOT NULL,
    tds_tcs VARCHAR(5),
    adjustment NUMERIC(18,2)
);

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
    reverse_charge BOOLEAN DEFAULT FALSE,
    subject TEXT,
    customer_notes TEXT,
    terms TEXT
);

CREATE TABLE IF NOT EXISTS tab_bill_items (
    item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    bill_id INT,
    item_details TEXT,
    account_id INT,
    quantity NUMERIC(18,2),
    rate NUMERIC(18,2),
    discount_pct NUMERIC(5,2),
    tax_id INT,
    customer_id INT,
    amount NUMERIC(18,2),
    tds_tcs VARCHAR(5),
    adjustment NUMERIC(18,2)
);

CREATE TABLE IF NOT EXISTS tab_bill_payment (
    bill_payment_id_internal SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    bill_payment_id INT,
    vendor_name VARCHAR(200),
    location VARCHAR(100),
    payment_number VARCHAR(50),
    payment_made NUMERIC(18,2),
    payment_date DATE,
    payment_mode VARCHAR(100),
    paid_through VARCHAR(200),
    reference_number VARCHAR(200),
    notes TEXT
);

CREATE TABLE IF NOT EXISTS tab_vendor_advanced (
    vendor_advance_id_internal SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    vendor_advance_id INT,
    vendor_name VARCHAR(200),
    gst_treatment VARCHAR(100),
    source_of_supply VARCHAR(50),
    destination_of_supply VARCHAR(50),
    location VARCHAR(100),
    payment_number VARCHAR(50),
    description_of_supply TEXT,
    payment_made NUMERIC(18,2),
    reverse_charge BOOLEAN DEFAULT FALSE,
    tds VARCHAR(50),
    payment_date DATE,
    payment_mode VARCHAR(100),
    paid_through VARCHAR(200),
    deposit_to VARCHAR(200),
    reference_number VARCHAR(200),
    notes TEXT
);

CREATE TABLE IF NOT EXISTS tab_vendor_credit (
    vendor_credit_id_internal SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    vendor_credit_id INT,
    vendor_name VARCHAR(200),
    location VARCHAR(100),
    credit_note_number VARCHAR(50),
    order_number VARCHAR(100),
    vendor_credit_date DATE,
    subject TEXT,
    accounts_payable VARCHAR(100),
    reverse_charge BOOLEAN DEFAULT FALSE,
    notes TEXT
);

CREATE TABLE IF NOT EXISTS tab_recurring_bill (
    recurring_bill_id_internal SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    recurring_bill_id INT,
    vendor_id INT,
    location VARCHAR(100),
    profile_name VARCHAR(200),
    repeat_every VARCHAR(50),
    start_on DATE,
    ends_on DATE,
    never_expires BOOLEAN DEFAULT FALSE,
    accounts_payable VARCHAR(200),
    payment_terms VARCHAR(200),
    reverse_charge_applicable BOOLEAN DEFAULT FALSE,
    warehouse_location VARCHAR(200),
    transaction_level VARCHAR(100),
    sub_total NUMERIC(18,2),
    discount NUMERIC(18,2),
    tds VARCHAR(100),
    adjustment NUMERIC(18,2),
    total_amount NUMERIC(18,2),
    notes TEXT
);

CREATE TABLE IF NOT EXISTS tab_recurring_bill_items (
    item_id_internal SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    item_id INT,
    recurring_bill_id INT,
    item_details TEXT,
    account VARCHAR(200),
    quantity NUMERIC(18,2),
    rate NUMERIC(18,2),
    tax VARCHAR(100),
    customer_details VARCHAR(200),
    amount NUMERIC(18,2)
);

-----------------------------------------------------------
-- 6. JOURNALS, EXPENSES & BUDGET
-----------------------------------------------------------

CREATE TABLE IF NOT EXISTS tab_journal (
    journal_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    location_id INT,
    journal_date DATE NOT NULL,
    journal_number VARCHAR(20) NOT NULL,
    reference_no VARCHAR(50),
    notes TEXT,
    reporting_method VARCHAR(20) NOT NULL,
    currency VARCHAR(10) NOT NULL,
    attachments TEXT
);

CREATE TABLE IF NOT EXISTS tab_journal_lines (
    line_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    journal_id INT,
    account_id INT,
    description TEXT,
    contact_id INT,
    debit NUMERIC(18,2),
    credit NUMERIC(18,2)
);

CREATE TABLE IF NOT EXISTS tab_budget (
    budget_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    name VARCHAR(150) NOT NULL,
    fiscal_year VARCHAR(20) NOT NULL,
    budget_period VARCHAR(20) NOT NULL,
    location_id INT,
    include_asset_liability_equity BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tab_record_expense (
    expense_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    location_id INT NOT NULL,
    expense_date DATE NOT NULL,
    expense_account_id INT NOT NULL,
    amount NUMERIC(18,2) NOT NULL,
    paid_through_account_id INT NOT NULL,
    expense_type VARCHAR(20) NOT NULL,
    sac VARCHAR(20),
    vendor_id INT,
    gst_treatment VARCHAR(50) NOT NULL,
    source_of_supply VARCHAR(50) NOT NULL,
    destination_of_supply VARCHAR(50) NOT NULL,
    reverse_charge BOOLEAN DEFAULT FALSE,
    tax_id INT,
    amount_is VARCHAR(20) NOT NULL,
    invoice_number VARCHAR(50),
    notes TEXT,
    customer_id INT
);

CREATE TABLE IF NOT EXISTS tab_record_mileage (
    mileage_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    location_id INT NOT NULL,
    mileage_date DATE NOT NULL,
    employee_id INT,
    calculation_method VARCHAR(20) NOT NULL,
    distance_km NUMERIC(10,2),
    odometer_start NUMERIC(10,2),
    odometer_end NUMERIC(10,2),
    amount NUMERIC(18,2) NOT NULL,
    paid_through_account_id INT NOT NULL,
    vendor_id INT,
    invoice_number VARCHAR(50),
    notes TEXT,
    customer_id INT
);

CREATE TABLE IF NOT EXISTS tab_recurring_expense (
    recurring_expense_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    profile_name VARCHAR(150) NOT NULL,
    location_id INT NOT NULL,
    repeat_every VARCHAR(20) NOT NULL,
    start_date DATE NOT NULL,
    ends_on DATE,
    never_expires BOOLEAN DEFAULT FALSE,
    expense_account_id INT NOT NULL,
    amount NUMERIC(18,2) NOT NULL,
    paid_through_account_id INT NOT NULL,
    expense_type VARCHAR(20) NOT NULL,
    sac VARCHAR(20),
    vendor_id INT,
    gst_treatment VARCHAR(50) NOT NULL,
    source_of_supply VARCHAR(50) NOT NULL,
    destination_of_supply VARCHAR(50) NOT NULL,
    reverse_charge BOOLEAN DEFAULT FALSE,
    tax_id INT,
    amount_is VARCHAR(20) NOT NULL,
    notes TEXT,
    customer_id INT
);


-----------------------------------------------------------
-- 7. TAXATION, ROLES & PERMISSIONS
-----------------------------------------------------------

CREATE TABLE IF NOT EXISTS tab_tax_exemption_master (
    tax_exemption_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    exemption_reason VARCHAR(200) NOT NULL,
    description TEXT,
    exemption_type VARCHAR(20) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tab_tax_preference_master (
    tax_preference_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    preference_code VARCHAR(30) NOT NULL UNIQUE,
    preference_name VARCHAR(100) NOT NULL,
    is_gst_applicable BOOLEAN DEFAULT TRUE,
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS tab_tax_master (
    tax_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tax_name VARCHAR(200),
    rate_percent NUMERIC(5,2),
    tax_type VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS tab_tax_group_master (
    tax_group_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    group_name VARCHAR(200) NOT NULL,
    description TEXT,
    supply_type VARCHAR(20),
    cgst_per NUMERIC(5,2),
    sgst_per NUMERIC(5,2),
    igst_per NUMERIC(5,2)
);

CREATE TABLE IF NOT EXISTS tab_tax_group_detail (
    tax_group_detail_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tax_group_id INT NOT NULL,
    tax_id INT NOT NULL,
    apply_order INT NOT NULL
);

CREATE TABLE IF NOT EXISTS tab_tcs_master (
    tcs_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tax_name VARCHAR(100) NOT NULL,
    rate NUMERIC(5,2) NOT NULL,
    nature_of_collection VARCHAR(100) NOT NULL,
    is_higher_rate BOOLEAN DEFAULT FALSE,
    start_date DATE,
    end_date DATE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tab_tds_master (
    tds_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tax_name VARCHAR(100) NOT NULL,
    rate NUMERIC(5,2) NOT NULL,
    section VARCHAR(50) NOT NULL,
    is_higher_rate BOOLEAN DEFAULT FALSE,
    start_date DATE,
    end_date DATE,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tab_transporter_master (
    transporter_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    transporter_name VARCHAR(200),
    gst_number VARCHAR(20),
    contact_person VARCHAR(100),
    phone VARCHAR(20),
    address TEXT
);

CREATE TABLE IF NOT EXISTS tab_user_master (
    user_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    username VARCHAR(100),
    password_hash VARCHAR(256),
    full_name VARCHAR(200),
    email VARCHAR(200),
    phone VARCHAR(20),
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS tab_role_master (
    role_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    role_name VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS tab_user_role (
    user_id INT NOT NULL,
    role_id INT NOT NULL,
    company_id INT NOT NULL
);

CREATE TABLE IF NOT EXISTS tabs (
    tab_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    tab_name VARCHAR(50) NOT NULL,
    tab_url VARCHAR(255),
    parent_id INT,
    icon_path VARCHAR(255),
    sort_order INT,
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS tab_role_permission (
    permission_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    role_id INT,
    tab_id INT NOT NULL,
    can_view BOOLEAN DEFAULT FALSE,
    can_add BOOLEAN DEFAULT FALSE,
    can_edit BOOLEAN DEFAULT FALSE,
    can_delete BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE
);

-----------------------------------------------------------
-- 8. VIEWS
-----------------------------------------------------------

CREATE OR REPLACE VIEW v_account AS
SELECT account_id, account_name, account_type, company_id FROM tab_account_master;

CREATE OR REPLACE VIEW v_accounts_payable AS
SELECT account_id AS accounts_payable_id, account_name, company_id 
FROM tab_account_master WHERE account_type = 'Liability';

CREATE OR REPLACE VIEW v_accounts_receivable_ref AS
SELECT account_id AS accounts_receivable, account_name, company_id
FROM tab_account_master WHERE account_type = 'Asset';

CREATE OR REPLACE VIEW v_customer AS
SELECT party_id AS customer_id, * FROM tab_party_master WHERE party_type = 'Customer';

CREATE OR REPLACE VIEW v_expense_account AS
SELECT account_id AS expense_account_id, account_name, company_id
FROM tab_account_master WHERE account_type = 'Expense';

CREATE OR REPLACE VIEW v_hsn_code_ref AS
SELECT hsn_code, company_id FROM tab_hsn_master;

CREATE OR REPLACE VIEW v_igst_ref AS
SELECT tax_id AS igst, company_id FROM tab_tax_master WHERE tax_type = 'IGST';

CREATE OR REPLACE VIEW v_paid_through_account AS
SELECT account_id AS paid_through_account_id, account_name, company_id
FROM tab_account_master WHERE account_type IN ('Cash','Bank');

CREATE OR REPLACE VIEW v_sac_ref AS
SELECT sac_code AS sac, company_id FROM tab_sac_master;

CREATE OR REPLACE VIEW v_vendor AS
SELECT party_id AS vendor_id, * FROM tab_party_master WHERE party_type = 'Vendor';

-----------------------------------------------------------
-- 9. INDEXES
-----------------------------------------------------------

CREATE INDEX idx_party_company ON tab_party_master(company_id);
CREATE INDEX idx_product_company ON tab_product_master(company_id);
CREATE INDEX idx_invoice_company ON tab_invoice(company_id);
CREATE INDEX idx_user_company ON tab_user_master(company_id);
CREATE INDEX idx_perms_role ON tab_role_permission(role_id);

