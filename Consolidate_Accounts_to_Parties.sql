-- MIGRATION SCRIPT: CONSOLIDATE ACCOUNT MASTER INTO PARTY MASTER
-- As per Team Leader's Methodology: 
-- party_type = account_type
-- company_name = account_name
-- display_name = account_code

BEGIN;

-- 1. Migrate existing Data from tab_account_master to tab_party_master
-- We temporarily disable triggers if any to ensure smooth migration
ALTER TABLE tab_party_master DISABLE TRIGGER ALL;

-- Insert accounts into party master
-- mapping: name -> company_name, type -> party_type, code -> display_name
INSERT INTO tab_party_master (company_id, party_type, company_name, display_name, created_on)
SELECT 
    company_id, 
    COALESCE(account_type, 'Account'), 
    account_name, 
    COALESCE(account_code, account_name) || ' (Account)', -- Added suffix to avoid unique constraint issues with existing parties
    CURRENT_TIMESTAMP
FROM tab_account_master;

-- 2. Update references in tab_ledger_settings
DO $$
DECLARE
    r RECORD;
    new_id INT;
BEGIN
    FOR r IN (SELECT * FROM tab_account_master) LOOP
        -- Find the new ID in party master by company and name
        SELECT party_id INTO new_id 
        FROM tab_party_master 
        WHERE company_id = r.company_id AND display_name = COALESCE(r.account_code, r.account_name) || ' (Account)'
        LIMIT 1;

        IF new_id IS NOT NULL THEN
            UPDATE tab_ledger_settings SET sales_revenue_account_id = new_id WHERE sales_revenue_account_id = r.account_id;
            UPDATE tab_ledger_settings SET purchase_expense_account_id = new_id WHERE purchase_expense_account_id = r.account_id;
            UPDATE tab_ledger_settings SET cgst_output_account_id = new_id WHERE cgst_output_account_id = r.account_id;
            UPDATE tab_ledger_settings SET sgst_output_account_id = new_id WHERE sgst_output_account_id = r.account_id;
            UPDATE tab_ledger_settings SET igst_output_account_id = new_id WHERE igst_output_account_id = r.account_id;
            UPDATE tab_ledger_settings SET cgst_input_account_id = new_id WHERE cgst_input_account_id = r.account_id;
            UPDATE tab_ledger_settings SET sgst_input_account_id = new_id WHERE sgst_input_account_id = r.account_id;
            UPDATE tab_ledger_settings SET igst_input_account_id = new_id WHERE igst_input_account_id = r.account_id;
            UPDATE tab_ledger_settings SET round_off_account_id = new_id WHERE round_off_account_id = r.account_id;
            
            -- Update Journal Entries
            UPDATE tab_jv SET dr_account_code = new_id WHERE dr_account_code = r.account_id;
            UPDATE tab_jv SET cr_account_code = new_id WHERE cr_account_code = r.account_id;
        END IF;
    END LOOP;
END $$;

ALTER TABLE tab_party_master ENABLE TRIGGER ALL;

-- 3. Cleanup: Drop the redundant table
-- DROP TABLE tab_account_master CASCADE; 
-- Note: I'll comment out the DROP temporarily for safety, user can uncomment after verification.

-- 4. Correct Foreign Key Constraints for Journal Entries
ALTER TABLE tab_jv DROP CONSTRAINT IF EXISTS fk_jv_dr_account;
ALTER TABLE tab_jv DROP CONSTRAINT IF EXISTS fk_jv_cr_account;
ALTER TABLE tab_jv ADD CONSTRAINT fk_jv_dr_party FOREIGN KEY (dr_account_code) REFERENCES tab_party_master(party_id);
ALTER TABLE tab_jv ADD CONSTRAINT fk_jv_cr_party FOREIGN KEY (cr_account_code) REFERENCES tab_party_master(party_id);

COMMIT;
