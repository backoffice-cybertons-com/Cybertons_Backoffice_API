-----------------------------------------------------------
-- SOFTBOOK POSTGRESQL TENANT DATABASE SCRIPT
-- GENERATED ON: 2026-01-29
-- DATABASE: SoftAccDb (Shared Tenant Database)
-----------------------------------------------------------

-- 2. CORE MASTERS
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
    is_current BOOLEAN DEFAULT FALSE,
    fiscal_year_label CHARACTER VARYING(20) COLLATE pg_catalog."default",
    CONSTRAINT uq_fiscal_year UNIQUE (company_id, start_date, end_date)
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
    payment_mode_id_internal SERIAL PRIMARY KEY,
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
-- 3. PARTY & CRM
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
    party_id INT, 
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
    micr VARCHAR(20),
    branch VARCHAR(200),
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

CREATE INDEX IF NOT EXISTS idx_party_company ON tab_party_master(company_id);
CREATE INDEX IF NOT EXISTS idx_product_company ON tab_product_master(company_id);
CREATE INDEX IF NOT EXISTS idx_invoice_company ON tab_invoice(company_id);
CREATE INDEX IF NOT EXISTS idx_user_company ON tab_user_master(company_id);
CREATE INDEX IF NOT EXISTS idx_perms_role ON tab_role_permission(role_id);

-----------------------------------------------------------
-- 10. FOREIGN KEY CONSTRAINTS (Selected)
-----------------------------------------------------------
-- Adding basic integrity constraints. Many schemas use logical IDs, but these help data quality.

DO $$ BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_address_party') THEN
        ALTER TABLE tab_party_address ADD CONSTRAINT fk_address_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_bank_party') THEN
        ALTER TABLE tab_party_bank_details ADD CONSTRAINT fk_bank_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_contact_party') THEN
        ALTER TABLE tab_party_contact_persons ADD CONSTRAINT fk_contact_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_tag_party') THEN
        ALTER TABLE tab_party_reporting_tags ADD CONSTRAINT fk_tag_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_invoice_customer') THEN
        ALTER TABLE tab_invoice ADD CONSTRAINT fk_invoice_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_quote_customer') THEN
        ALTER TABLE tab_quote ADD CONSTRAINT fk_quote_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_sales_order_customer') THEN
        ALTER TABLE tab_sales_order ADD CONSTRAINT fk_sales_order_customer FOREIGN KEY (customer_id) REFERENCES tab_party_master(party_id);
    END IF;
END $$;

-----------------------------------------------------------
-- 11. STORED PROCEDURES & FUNCTIONS
-----------------------------------------------------------

-- User Validation
CREATE OR REPLACE FUNCTION sp_validate_user(
    p_username VARCHAR,
    p_password_hash VARCHAR
)
RETURNS SETOF tab_user_master AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tab_user_master
    WHERE (username = p_username OR email = p_username)
      AND TRIM(password_hash) = TRIM(p_password_hash);
END;
$$ LANGUAGE plpgsql;

-- User Registration
CREATE OR REPLACE FUNCTION sp_user_register(
    p_username VARCHAR,
    p_password_hash VARCHAR,
    p_email VARCHAR,
    p_full_name VARCHAR DEFAULT NULL,
    p_phone VARCHAR DEFAULT NULL
)
RETURNS SETOF tab_user_master AS $$
DECLARE
    v_new_user_id INT;
BEGIN
    -- Check for existing username or email
    IF EXISTS (SELECT 1 FROM tab_user_master WHERE username = p_username) THEN
        RAISE EXCEPTION 'Username already exists';
    END IF;

    IF EXISTS (SELECT 1 FROM tab_user_master WHERE email = p_email) THEN
        RAISE EXCEPTION 'Email already registered';
    END IF;

    -- Insert new user
    INSERT INTO tab_user_master (username, password_hash, email, full_name, phone, is_active, company_id)
    VALUES (p_username, p_password_hash, p_email, p_full_name, p_phone, TRUE, 0) -- Assuming company_id 0 or handled by context
    RETURNING user_id INTO v_new_user_id;

    -- Return the new user row
    RETURN QUERY SELECT * FROM tab_user_master WHERE user_id = v_new_user_id;
END;
$$ LANGUAGE plpgsql;

-- Get User By Email
CREATE OR REPLACE FUNCTION sp_user_get_by_email(
    p_email VARCHAR
)
RETURNS SETOF tab_user_master AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tab_user_master
    WHERE email = p_email;
END;
$$ LANGUAGE plpgsql;

-- Update Password
CREATE OR REPLACE FUNCTION sp_user_update_password(
    p_email VARCHAR,
    p_new_password_hash VARCHAR
)
RETURNS VOID AS $$
BEGIN
    UPDATE tab_user_master
    SET password_hash = p_new_password_hash
    WHERE email = p_email;
END;
$$ LANGUAGE plpgsql;

-- Sidebar and Permissions
CREATE OR REPLACE FUNCTION usp_get_user_sidebar_and_permissions(
    p_user_id INT
)
RETURNS TABLE (
    role_id INT,
    role_name VARCHAR,
    tab_id INT,
    tab_name VARCHAR,
    tab_url VARCHAR,
    parent_id INT,
    icon_path VARCHAR,
    sort_order INT,
    can_view BOOLEAN,
    can_add BOOLEAN,
    can_edit BOOLEAN,
    can_delete BOOLEAN
) AS $$
DECLARE
    v_role_id INT;
BEGIN
    -- 1. Get the role for this user
    SELECT ur.role_id INTO v_role_id
    FROM tab_user_role ur
    JOIN tab_role_master rm ON ur.role_id = rm.role_id
    WHERE ur.user_id = p_user_id
        AND rm.is_active = TRUE
        AND rm.is_deleted = FALSE
    LIMIT 1;

    IF v_role_id IS NULL THEN
        RAISE EXCEPTION 'No active role assigned to this user.';
    END IF;

    -- 2. Fetch sidebar + permissions
    RETURN QUERY
    SELECT  
        rm.role_id,
        rm.role_name,
        t.tab_id,
        t.tab_name,
        t.tab_url,
        t.parent_id,
        t.icon_path,
        t.sort_order,
        p.can_view,
        p.can_add,
        p.can_edit,
        p.can_delete
    FROM tab_role_permission p
    JOIN tabs t ON p.tab_id = t.tab_id
    JOIN tab_role_master rm ON p.role_id = rm.role_id
    WHERE 
        p.role_id = v_role_id
        AND p.can_view = TRUE
        AND p.is_active = TRUE
        AND t.is_active = TRUE
        AND rm.is_active = TRUE
        AND rm.is_deleted = FALSE
    ORDER BY 
        t.parent_id,
        t.sort_order,
        t.tab_name;
END;
$$ LANGUAGE plpgsql;
-- Product Create
CREATE OR REPLACE FUNCTION sp_product_create(
    p_company_id INT,
    p_product_type VARCHAR,
    p_product_name VARCHAR,
    p_selling_price NUMERIC,
    p_sales_account VARCHAR,
    p_sales_description VARCHAR,
    p_cost_price NUMERIC,
    p_purchase_account VARCHAR,
    p_purchase_description VARCHAR,
    p_preferred_vendor_id INT,
    p_sellable BOOLEAN,
    p_purchasable BOOLEAN,
    p_unit_id INT,
    p_hsn_id INT,
    p_sac_id INT,
    p_tax_preference_id INT,
    p_tax_group_id INT,
    p_product_group_id INT,
    p_product_packing_unit INT,
    p_opening_quantity NUMERIC,
    p_opening_price NUMERIC,
    p_opening_amount NUMERIC,
    p_purchase_effected_account INT,
    p_sale_effected_account INT,
    p_selling_price_2 NUMERIC,
    p_selling_price_3 NUMERIC,
    p_selling_price_4 NUMERIC,
    p_selling_price_5 NUMERIC,
    p_min_quantity NUMERIC,
    p_max_quantity NUMERIC,
    p_re_order_quantity NUMERIC,
    p_tax_exemption_id INT
)
RETURNS VOID AS $$
BEGIN
    INSERT INTO tab_product_master (
        company_id, product_type, product_name, selling_price, sales_account, sales_description, 
        cost_price, purchase_account, purchase_description, preferred_vendor_id, 
        sellable, purchasable, unit_id, hsn_id, sac_id, tax_preference_id, tax_group_id,
        product_group_id, product_packing_unit, opening_quantity, opening_price, opening_amount,
        purchase_effected_account, sale_effected_account, selling_price_2, selling_price_3,
        selling_price_4, selling_price_5, min_quantity, max_quantity, re_order_quantity, tax_exemption_id
    )
    VALUES (
        p_company_id, p_product_type, p_product_name, p_selling_price, p_sales_account, p_sales_description,
        p_cost_price, p_purchase_account, p_purchase_description, p_preferred_vendor_id,
        COALESCE(p_sellable, TRUE), COALESCE(p_purchasable, TRUE), p_unit_id, p_hsn_id, p_sac_id, p_tax_preference_id, p_tax_group_id,
        p_product_group_id, p_product_packing_unit, p_opening_quantity, p_opening_price, p_opening_amount,
        p_purchase_effected_account, p_sale_effected_account, p_selling_price_2, p_selling_price_3,
        p_selling_price_4, p_selling_price_5, p_min_quantity, p_max_quantity, p_re_order_quantity, p_tax_exemption_id
    );
END;
$$ LANGUAGE plpgsql;

-- Product Update
CREATE OR REPLACE FUNCTION sp_product_update(
    p_product_id INT,
    p_company_id INT,
    p_product_type VARCHAR,
    p_product_name VARCHAR,
    p_selling_price NUMERIC,
    p_sales_account VARCHAR,
    p_sales_description VARCHAR,
    p_cost_price NUMERIC,
    p_purchase_account VARCHAR,
    p_purchase_description VARCHAR,
    p_preferred_vendor_id INT,
    p_sellable BOOLEAN,
    p_purchasable BOOLEAN,
    p_unit_id INT,
    p_hsn_id INT,
    p_sac_id INT,
    p_tax_preference_id INT,
    p_tax_group_id INT,
    p_product_group_id INT,
    p_product_packing_unit INT,
    p_opening_quantity NUMERIC,
    p_opening_price NUMERIC,
    p_opening_amount NUMERIC,
    p_purchase_effected_account INT,
    p_sale_effected_account INT,
    p_selling_price_2 NUMERIC,
    p_selling_price_3 NUMERIC,
    p_selling_price_4 NUMERIC,
    p_selling_price_5 NUMERIC,
    p_min_quantity NUMERIC,
    p_max_quantity NUMERIC,
    p_re_order_quantity NUMERIC,
    p_tax_exemption_id INT
)
RETURNS VOID AS $$
BEGIN
    UPDATE tab_product_master
    SET 
        product_type = p_product_type,
        product_name = p_product_name,
        selling_price = p_selling_price,
        sales_account = p_sales_account,
        sales_description = p_sales_description,
        cost_price = p_cost_price,
        purchase_account = p_purchase_account,
        purchase_description = p_purchase_description,
        preferred_vendor_id = p_preferred_vendor_id,
        sellable = p_sellable,
        purchasable = p_purchasable,
        unit_id = p_unit_id,
        hsn_id = p_hsn_id,
        sac_id = p_sac_id,
        tax_preference_id = p_tax_preference_id,
        tax_group_id = p_tax_group_id,
        product_group_id = p_product_group_id,
        product_packing_unit = p_product_packing_unit,
        opening_quantity = p_opening_quantity,
        opening_price = p_opening_price,
        opening_amount = p_opening_amount,
        purchase_effected_account = p_purchase_effected_account,
        sale_effected_account = p_sale_effected_account,
        selling_price_2 = p_selling_price_2,
        selling_price_3 = p_selling_price_3,
        selling_price_4 = p_selling_price_4,
        selling_price_5 = p_selling_price_5,
        min_quantity = p_min_quantity,
        max_quantity = p_max_quantity,
        re_order_quantity = p_re_order_quantity,
        tax_exemption_id = p_tax_exemption_id
    WHERE product_id = p_product_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Product Delete
CREATE OR REPLACE FUNCTION sp_product_delete(
    p_product_id INT,
    p_company_id INT
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM tab_product_master
    WHERE product_id = p_product_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get All Products (Basic)
CREATE OR REPLACE FUNCTION sp_product_get_all(
    p_company_id INT
)
RETURNS SETOF tab_product_master AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_product_master
    WHERE company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get Product By ID
CREATE OR REPLACE FUNCTION sp_product_get_by_id(
    p_product_id INT,
    p_company_id INT
)
RETURNS SETOF tab_product_master AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM tab_product_master
    WHERE product_id = p_product_id AND company_id = p_company_id;
END;
$$ LANGUAGE plpgsql;

-- Get All Products with Details (Joins)
CREATE OR REPLACE FUNCTION public.sp_product_get_all_with_details(
	p_company_id integer)
    RETURNS TABLE(product_id integer, product_name character varying, product_type character varying, selling_price numeric, sales_description text, purchase_description text, cost_price numeric, tax_group_id integer, tax_preference_id integer, base_unit character varying, packing_unit character varying, product_group_name character varying, hsn_code character varying, sac_code character varying, tax_group_name character varying, min_quantity numeric, max_quantity numeric, re_order_quantity numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        P.product_id,
        P.product_name,
        P.product_type,

        P.selling_price,
        P.sales_description,
        P.purchase_description,
        P.cost_price,

        P.tax_group_id,
        P.tax_preference_id,

        U.unit_name AS base_unit,
        PU.unit_name AS packing_unit,

        PG.product_group_name,

        H.hsn_code,
        S.sac_code,

        TG.group_name AS tax_group_name,

        P.min_quantity,
        P.max_quantity,
        P.re_order_quantity

    FROM tab_product_master P

    LEFT JOIN tab_unit_master U
        ON P.unit_id = U.unit_id

    LEFT JOIN tab_unit_master PU
        ON P.product_packing_unit = PU.unit_id

    LEFT JOIN tab_product_group_master PG
        ON P.product_group_id = PG.product_group_id

    LEFT JOIN tab_hsn_master H
        ON P.hsn_id = H.hsn_id

    LEFT JOIN tab_sac_master S
        ON P.sac_id = S.sac_id

    LEFT JOIN tab_tax_group_master TG
        ON P.tax_group_id = TG.tax_group_id

    WHERE P.company_id = p_company_id

    ORDER BY P.product_id DESC;
END;
$BODY$;

ALTER FUNCTION public.sp_product_get_all_with_details(integer)
OWNER TO postgres;

-- Unified Product Function for Repository
CREATE OR REPLACE FUNCTION fn_product_master(
    p_action VARCHAR,
    p_company_id INT,
    p_product_id INT DEFAULT NULL,
    p_product_type VARCHAR DEFAULT NULL,
    p_product_name VARCHAR DEFAULT NULL,
    p_selling_price NUMERIC DEFAULT NULL,
    p_sales_account VARCHAR DEFAULT NULL,
    p_sales_description VARCHAR DEFAULT NULL,
    p_cost_price NUMERIC DEFAULT NULL,
    p_purchase_account VARCHAR DEFAULT NULL,
    p_purchase_description VARCHAR DEFAULT NULL,
    p_preferred_vendor_id INT DEFAULT NULL,
    p_sellable BOOLEAN DEFAULT NULL,
    p_purchasable BOOLEAN DEFAULT NULL,
    p_unit_id INT DEFAULT NULL,
    p_hsn_id INT DEFAULT NULL,
    p_sac_id INT DEFAULT NULL,
    p_tax_preference_id INT DEFAULT NULL,
    p_tax_group_id INT DEFAULT NULL,
    p_product_group_id INT DEFAULT NULL,
    p_product_packing_unit INT DEFAULT NULL,
    p_opening_quantity NUMERIC DEFAULT NULL,
    p_opening_price NUMERIC DEFAULT NULL,
    p_opening_amount NUMERIC DEFAULT NULL,
    p_purchase_effected_account INT DEFAULT NULL,
    p_sale_effected_account INT DEFAULT NULL,
    p_selling_price_2 NUMERIC DEFAULT NULL,
    p_selling_price_3 NUMERIC DEFAULT NULL,
    p_selling_price_4 NUMERIC DEFAULT NULL,
    p_selling_price_5 NUMERIC DEFAULT NULL,
    p_min_quantity NUMERIC DEFAULT NULL,
    p_max_quantity NUMERIC DEFAULT NULL,
    p_re_order_quantity NUMERIC DEFAULT NULL,
    p_tax_exemption_id INT DEFAULT NULL
)
RETURNS SETOF tab_product_master AS $$
BEGIN
    IF p_action = 'GETALL' THEN
        RETURN QUERY SELECT * FROM sp_product_get_all(p_company_id);
    ELSIF p_action = 'GETBYID' THEN
        RETURN QUERY SELECT * FROM sp_product_get_by_id(p_product_id, p_company_id);
    ELSIF p_action = 'INSERT' THEN
        PERFORM sp_product_create(
            p_company_id, p_product_type, p_product_name, p_selling_price, p_sales_account, p_sales_description,
            p_cost_price, p_purchase_account, p_purchase_description, p_preferred_vendor_id,
            p_sellable, p_purchasable, p_unit_id, p_hsn_id, p_sac_id, p_tax_preference_id, p_tax_group_id,
            p_product_group_id, p_product_packing_unit, p_opening_quantity, p_opening_price, p_opening_amount,
            p_purchase_effected_account, p_sale_effected_account, p_selling_price_2, p_selling_price_3,
            p_selling_price_4, p_selling_price_5, p_min_quantity, p_max_quantity, p_re_order_quantity, p_tax_exemption_id
        );
        RETURN;
    ELSIF p_action = 'UPDATE' THEN
        PERFORM sp_product_update(
            p_product_id, p_company_id, p_product_type, p_product_name, p_selling_price, p_sales_account, p_sales_description,
            p_cost_price, p_purchase_account, p_purchase_description, p_preferred_vendor_id,
            p_sellable, p_purchasable, p_unit_id, p_hsn_id, p_sac_id, p_tax_preference_id, p_tax_group_id,
            p_product_group_id, p_product_packing_unit, p_opening_quantity, p_opening_price, p_opening_amount,
            p_purchase_effected_account, p_sale_effected_account, p_selling_price_2, p_selling_price_3,
            p_selling_price_4, p_selling_price_5, p_min_quantity, p_max_quantity, p_re_order_quantity, p_tax_exemption_id
        );
        RETURN;
    ELSIF p_action = 'DELETE' THEN
        PERFORM sp_product_delete(p_product_id, p_company_id);
        RETURN;
    END IF;
END;
$$ LANGUAGE plpgsql;

ALTER FUNCTION public.sp_product_get_all_with_details(integer)
OWNER TO postgres;

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

-- Fiscal Year Master - Overlap Validation + Current Year Flag
CREATE OR REPLACE FUNCTION public.fn_fiscal_year(
    p_action TEXT,
    p_fiscal_year_id INTEGER DEFAULT NULL,
    p_company_id INTEGER DEFAULT NULL,
    p_start_date DATE DEFAULT NULL,
    p_end_date DATE DEFAULT NULL,
    p_is_current BOOLEAN DEFAULT FALSE
)
RETURNS SETOF tab_fiscal_year_master
LANGUAGE plpgsql
AS $$
DECLARE
    v_label TEXT;
BEGIN

    v_label := TO_CHAR(p_start_date, 'YYYY') || '-' ||
               RIGHT(TO_CHAR(p_end_date, 'YYYY'), 2);

    IF p_action = 'INSERT' THEN

        IF p_start_date >= p_end_date THEN
            RAISE EXCEPTION 'Start date must be less than end date';
        END IF;

        IF EXISTS (
            SELECT 1 FROM tab_fiscal_year_master
            WHERE company_id = p_company_id
            AND (
                p_start_date BETWEEN start_date AND end_date OR
                p_end_date BETWEEN start_date AND end_date OR
                start_date BETWEEN p_start_date AND p_end_date
            )
        ) THEN
            RAISE EXCEPTION 'Fiscal year overlaps with existing year';
        END IF;

        RETURN QUERY
        INSERT INTO tab_fiscal_year_master (
            company_id,
            start_date,
            end_date,
            is_current,
            fiscal_year_label
        )
        VALUES (
            p_company_id,
            p_start_date,
            p_end_date,
            p_is_current,
            v_label
        )
        RETURNING *;

        IF p_is_current THEN
            UPDATE tab_fiscal_year_master
            SET is_current = FALSE
            WHERE company_id = p_company_id
            AND fiscal_year_id NOT IN (
                SELECT fiscal_year_id FROM tab_fiscal_year_master
                WHERE company_id = p_company_id
                ORDER BY fiscal_year_id DESC LIMIT 1
            );
        END IF;

    ELSIF p_action = 'UPDATE' THEN

        IF EXISTS (
            SELECT 1 FROM tab_fiscal_year_master
            WHERE company_id = p_company_id
            AND fiscal_year_id <> p_fiscal_year_id
            AND (
                p_start_date BETWEEN start_date AND end_date OR
                p_end_date BETWEEN start_date AND end_date OR
                start_date BETWEEN p_start_date AND p_end_date
            )
        ) THEN
            RAISE EXCEPTION 'Fiscal year overlaps with existing year';
        END IF;

        RETURN QUERY
        UPDATE tab_fiscal_year_master
        SET
            start_date = COALESCE(p_start_date, start_date),
            end_date = COALESCE(p_end_date, end_date),
            is_current = COALESCE(p_is_current, is_current),
            fiscal_year_label = v_label
        WHERE fiscal_year_id = p_fiscal_year_id
        AND company_id = p_company_id
        RETURNING *;

        IF p_is_current THEN
            UPDATE tab_fiscal_year_master
            SET is_current = FALSE
            WHERE company_id = p_company_id
            AND fiscal_year_id <> p_fiscal_year_id;
        END IF;

    ELSIF p_action = 'DELETE' THEN
        DELETE FROM tab_fiscal_year_master
        WHERE fiscal_year_id = p_fiscal_year_id
        AND company_id = p_company_id;
        RETURN;

    ELSIF p_action = 'GETBYID' THEN
        RETURN QUERY
        SELECT *
        FROM tab_fiscal_year_master
        WHERE fiscal_year_id = p_fiscal_year_id
        AND company_id = p_company_id;

    ELSIF p_action = 'GETALL' THEN
        RETURN QUERY
        SELECT *
        FROM tab_fiscal_year_master
        WHERE company_id = p_company_id
        ORDER BY start_date DESC;

    END IF;

END;
$$;

-- PostgreSQL Universal Master Procedure (PL/pgSQL Function) - Snake Case Version
-- This function handles CRUD for all 27 Master Entities with mandatory company_id isolation.

DROP FUNCTION IF EXISTS sp_master_universal(VARCHAR(100), VARCHAR(20), INT, INT, JSONB);
DROP FUNCTION IF EXISTS sp_master_universal(VARCHAR, VARCHAR, INT, INT, JSONB);
CREATE OR REPLACE FUNCTION sp_master_universal(
    p_master_name VARCHAR(100),
    p_action VARCHAR(20),
    p_company_id INT,
    p_id INT DEFAULT NULL,
    p_json_data JSONB DEFAULT NULL
) 
RETURNS SETOF JSON AS $$
DECLARE
    v_table_name TEXT;
    v_pk_name TEXT;
    v_query TEXT;
BEGIN
    -- Mapping logical name to actual snake_case table name and primary key
    CASE LOWER(p_master_name)
        WHEN 'account', 'tabaccountmaster', 'v_account' THEN v_table_name := 'tab_account_master'; v_pk_name := 'account_id';
        WHEN 'accountspayable', 'v_accountspayable' THEN v_table_name := 'v_accounts_payable'; v_pk_name := 'accounts_payable_id';
        WHEN 'accountsreceivableref', 'v_accountsreceivableref' THEN v_table_name := 'v_accounts_receivable_ref'; v_pk_name := 'accounts_receivable';
        WHEN 'bank', 'tabbankmaster' THEN v_table_name := 'tab_bank_master'; v_pk_name := 'bank_master_id';
        WHEN 'branch', 'tabbranchmaster' THEN v_table_name := 'tab_branch_master'; v_pk_name := 'branch_id';
        WHEN 'customer', 'v_customer' THEN v_table_name := 'v_customer'; v_pk_name := 'customer_id';
        WHEN 'expenseaccount', 'v_expenseaccount' THEN v_table_name := 'v_expense_account'; v_pk_name := 'expense_account_id';
        WHEN 'hsncoderef', 'v_hsncoderef' THEN v_table_name := 'v_hsn_code_ref'; v_pk_name := 'hsn_code';
        WHEN 'igstref', 'v_igstref' THEN v_table_name := 'v_igst_ref'; v_pk_name := 'igst';
        WHEN 'paidthroughaccount', 'v_paidthroughaccount' THEN v_table_name := 'v_paid_through_account'; v_pk_name := 'paid_through_account_id';
        WHEN 'sacref', 'v_sacref' THEN v_table_name := 'v_sac_ref'; v_pk_name := 'sac';
        WHEN 'vendor', 'v_vendor' THEN v_table_name := 'v_vendor'; v_pk_name := 'vendor_id';
        WHEN 'unit', 'tabunitmaster' THEN v_table_name := 'tab_unit_master'; v_pk_name := 'unit_id';
        WHEN 'tax', 'tabtaxmaster' THEN v_table_name := 'tab_tax_master'; v_pk_name := 'tax_id';
        WHEN 'hsn', 'tabhsnmaster' THEN v_table_name := 'tab_hsn_master'; v_pk_name := 'hsn_id';
        WHEN 'sac', 'tabsacmaster' THEN v_table_name := 'tab_sac_master'; v_pk_name := 'sac_id';
        WHEN 'currency', 'tabcurrencymaster' THEN v_table_name := 'tab_currency_master'; v_pk_name := 'currency_id';
        WHEN 'godown', 'tabgodownmaster' THEN v_table_name := 'tab_godown_master'; v_pk_name := 'godown_id';
        WHEN 'warehouse', 'tabwarehousemaster' THEN v_table_name := 'tab_warehouse_master'; v_pk_name := 'warehouse_id';
        WHEN 'costcenter', 'tabcostcentermaster' THEN v_table_name := 'tab_cost_center_master'; v_pk_name := 'cost_center_id';
        WHEN 'fiscalyear', 'tabfiscalyearmaster' THEN v_table_name := 'tab_fiscal_year_master'; v_pk_name := 'fiscal_year_id';
        WHEN 'paymentmode', 'tabpaymentmodemaster' THEN v_table_name := 'tab_payment_mode_master'; v_pk_name := 'payment_mode_id_internal';
        WHEN 'paymentterm', 'tabpaymenttermmaster' THEN v_table_name := 'tab_payment_term_master'; v_pk_name := 'term_id';
        WHEN 'productgroup', 'tabproductgroupmaster' THEN v_table_name := 'tab_product_group_master'; v_pk_name := 'product_group_id';
        WHEN 'project', 'tabprojectmaster' THEN v_table_name := 'tab_project_master'; v_pk_name := 'project_id';
        WHEN 'role', 'tabrolemaster' THEN v_table_name := 'tab_role_master'; v_pk_name := 'role_id';
        WHEN 'salesperson', 'tabsalespersonmaster' THEN v_table_name := 'tab_salesperson_master'; v_pk_name := 'salesperson_id';
        WHEN 'transporter', 'tabtransportermaster' THEN v_table_name := 'tab_transporter_master'; v_pk_name := 'transporter_id';
        WHEN 'partytype', 'tabpartytypemaster' THEN v_table_name := 'tab_party_type_master'; v_pk_name := 'party_type_id';
        WHEN 'taxgroup', 'tabtaxgroupmaster' THEN v_table_name := 'tab_tax_group_master'; v_pk_name := 'tax_group_id';
        WHEN 'taxgroupdetail', 'tabtaxgroupdetail' THEN v_table_name := 'tab_tax_group_detail'; v_pk_name := 'tax_group_detail_id';
        WHEN 'tcs', 'tabtcsmaster' THEN v_table_name := 'tab_tcs_master'; v_pk_name := 'tcs_id';
        WHEN 'tds', 'tabtdsmaster' THEN v_table_name := 'tab_tds_master'; v_pk_name := 'tds_id';
        WHEN 'user', 'tabusermaster' THEN v_table_name := 'tab_user_master'; v_pk_name := 'user_id';
        WHEN 'addresstype', 'tabaddresstypemaster' THEN v_table_name := 'tab_address_type_master'; v_pk_name := 'address_type_id';
        WHEN 'taxexemption', 'tabtaxexemptionmaster' THEN v_table_name := 'tab_tax_exemption_master'; v_pk_name := 'tax_exemption_id';
        WHEN 'taxpreference', 'tabtaxpreferencemaster' THEN v_table_name := 'tab_tax_preference_master'; v_pk_name := 'tax_preference_id';
        ELSE RAISE EXCEPTION 'Master % not supported', p_master_name;
    END CASE;

    -- Dynamic CRUD Implementation
    IF p_action = 'SELECT' THEN
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.company_id = %L', v_table_name, p_company_id);
        
    ELSIF p_action = 'SELECT_BY_ID' THEN
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.%I = %L AND t.company_id = %L', v_table_name, v_pk_name, p_id, p_company_id);
    
    ELSIF p_action = 'SELECT_BY_FILTER' AND LOWER(p_master_name) IN ('taxgroupdetail', 'tabtaxgroupdetail') THEN
        -- Special handling for TaxGroupDetail filtering by TaxGroupId
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.tax_group_id = %L AND t.company_id = %L', v_table_name, p_id, p_company_id);

    ELSIF p_action = 'INSERT' THEN
        -- Special handling: fiscal year uses validation + overlap checks
        IF LOWER(p_master_name) IN ('fiscalyear', 'tabfiscalyearmaster', 'fiscal_year', 'fiscal_year_master', 'fiscalyears') THEN
            RETURN QUERY
            SELECT row_to_json(t)
            FROM public.fn_fiscal_year(
                'INSERT',
                NULL,
                p_company_id,
                (p_json_data->>'start_date')::date,
                (p_json_data->>'end_date')::date,
                COALESCE((p_json_data->>'is_current')::boolean, FALSE)
            ) t;
            RETURN;
        END IF;

        -- Prevent insertion into views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot insert into view %', v_table_name; END IF;
        -- Standard insertion with company_id enforcement
        v_query := format('INSERT INTO %I SELECT * FROM jsonb_populate_record(NULL::%I, %L || jsonb_build_object(''company_id'', %L)) RETURNING row_to_json(%I)', 
                         v_table_name, v_table_name, p_json_data, p_company_id, v_table_name);
        RETURN QUERY EXECUTE v_query;
        
    ELSIF p_action = 'UPDATE' THEN
        -- Special handling: fiscal year uses validation + overlap checks
        IF LOWER(p_master_name) IN ('fiscalyear', 'tabfiscalyearmaster', 'fiscal_year', 'fiscal_year_master', 'fiscalyears') THEN
            RETURN QUERY
            SELECT row_to_json(t)
            FROM public.fn_fiscal_year(
                'UPDATE',
                p_id,
                p_company_id,
                (p_json_data->>'start_date')::date,
                (p_json_data->>'end_date')::date,
                COALESCE((p_json_data->>'is_current')::boolean, FALSE)
            ) t;
            RETURN;
        END IF;

        -- Prevent update on views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot update view %', v_table_name; END IF;
        v_query := format('UPDATE %I t SET (%I) = (SELECT %I FROM jsonb_populate_record(NULL::%I, %L)) 
                          WHERE t.%I = %L AND t.company_id = %L RETURNING row_to_json(t)', 
                         v_table_name, v_pk_name, v_pk_name, v_table_name, p_json_data, v_pk_name, p_id, p_company_id);
        RETURN QUERY EXECUTE v_query;
                       
    ELSIF p_action = 'DELETE' THEN
        -- Prevent delete on views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot delete from view %', v_table_name; END IF;
        EXECUTE format('DELETE FROM %I WHERE %I = %L AND company_id = %L', v_table_name, v_pk_name, p_id, p_company_id);
        RETURN NEXT json_build_object('status', 'deleted', 'id', p_id);
    END IF;

END;
$$ LANGUAGE plpgsql;
