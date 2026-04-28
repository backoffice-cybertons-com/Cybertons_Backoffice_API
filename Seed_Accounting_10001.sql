-- FIX & SEED SCRIPT for Company 10001
-- Run this in pgAdmin or your SQL tool to initialize accounting settings

DO $$ 
BEGIN
    -- 1. Ensure tab_ledger_settings is ready for mappings
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='sales_revenue_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN sales_revenue_account_id INT;
        ALTER TABLE tab_ledger_settings ADD COLUMN purchase_expense_account_id INT;
        ALTER TABLE tab_ledger_settings ADD COLUMN cgst_output_account_id INT;
        ALTER TABLE tab_ledger_settings ADD COLUMN sgst_output_account_id INT;
        ALTER TABLE tab_ledger_settings ADD COLUMN igst_output_account_id INT;
        ALTER TABLE tab_ledger_settings ADD COLUMN cgst_input_account_id INT;
        ALTER TABLE tab_ledger_settings ADD COLUMN sgst_input_account_id INT;
        ALTER TABLE tab_ledger_settings ADD COLUMN igst_input_account_id INT;
        ALTER TABLE tab_ledger_settings ADD COLUMN round_off_account_id INT;
    END IF;

    -- 2. Seed Account Groups, Voucher Types, and Default Accounts for Company 10001
    -- This will also link them in tab_ledger_settings automatically.
    PERFORM sp_seed_company_accounting_data(10001);

    RAISE NOTICE 'Accounting data seeded and mapped for Company 10001 successfully.';
END $$;

-- 3. Verify the mappings
SELECT * FROM tab_ledger_settings WHERE company_id = 10001; 
