-- SQL SCRIPT: ADD TRANSACTION MASTERS & LEDGER SETTINGS
-- RUN THIS SCRIPT TO ENABLE ACCOUNTING TRANSACTIONS

-- 1. Account Group Master
CREATE TABLE IF NOT EXISTS tab_account_group_master (
    group_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    group_name VARCHAR(150) NOT NULL,
    parent_group_id INT,
    group_type VARCHAR(50) NOT NULL, -- Asset, Liability, Income, Expense
    is_active BOOLEAN DEFAULT TRUE,
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_account_group_parent FOREIGN KEY (parent_group_id) REFERENCES tab_account_group_master(group_id),
    CONSTRAINT uq_account_group_name UNIQUE (company_id, group_name)
);
CREATE INDEX IF NOT EXISTS idx_acc_group_company ON tab_account_group_master(company_id);

-- 2. Voucher Type Master
CREATE TABLE IF NOT EXISTS tab_voucher_type_master (
    v_type_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    v_type_name VARCHAR(100) NOT NULL, -- Sales, Purchase, Journal, Payment, Receipt, Contra
    prefix VARCHAR(20),
    suffix VARCHAR(20),
    starting_no INT DEFAULT 1,
    is_auto_number BOOLEAN DEFAULT TRUE,
    is_active BOOLEAN DEFAULT TRUE,
    created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_v_type_name UNIQUE (company_id, v_type_name)
);
CREATE INDEX IF NOT EXISTS idx_v_type_company ON tab_voucher_type_master(company_id);

-- 3. Expand Ledger Settings for Automated Journal Entries
DO $$
BEGIN
    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN sales_revenue_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;

    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN purchase_expense_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;

    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN cgst_output_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;

    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN sgst_output_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;

    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN igst_output_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;

    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN cgst_input_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;

    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN sgst_input_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;

    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN igst_input_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;

    BEGIN
        ALTER TABLE tab_ledger_settings ADD COLUMN round_off_account_id INT;
    EXCEPTION WHEN duplicate_column THEN END;
END $$;
