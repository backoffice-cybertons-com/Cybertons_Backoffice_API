-- FIX SCRIPT: Update tab_ledger_settings and re-seed data
-- Run this script in your PostgreSQL database

-- 1. Update tab_ledger_settings schema
DO $$ 
BEGIN 
    -- Add missing account mapping columns if they don't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='sales_revenue_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN sales_revenue_account_id BIGINT;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='purchase_expense_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN purchase_expense_account_id BIGINT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='cgst_output_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN cgst_output_account_id BIGINT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='sgst_output_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN sgst_output_account_id BIGINT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='igst_output_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN igst_output_account_id BIGINT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='cgst_input_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN cgst_input_account_id BIGINT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='sgst_input_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN sgst_input_account_id BIGINT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='igst_input_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN igst_input_account_id BIGINT;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='tab_ledger_settings' AND column_name='round_off_account_id') THEN
        ALTER TABLE tab_ledger_settings ADD COLUMN round_off_account_id BIGINT;
    END IF;
END $$;

-- 2. Ensure tab_jv exists (using the user's provided schema)
-- We won't drop it if it exists, just ensure columns match.
-- The user already has tab_jv, so we skip recreation.

-- 3. Re-seed accounting data for Company ID 1
-- This will create the accounts and map them in tab_ledger_settings
CALL sp_seed_company_accounting_data(1);

-- 4. Verify mappings
SELECT * FROM tab_ledger_settings WHERE company_id = 1;
