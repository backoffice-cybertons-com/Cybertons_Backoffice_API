-- SQL SCRIPT: APPLY MASTER CONSTRAINTS FOR MULTI-TENANCY
-- This script prevents duplicate records for the same company.

-- 1. Account Master
ALTER TABLE tab_account_master ADD CONSTRAINT uq_account_code UNIQUE (company_id, account_code);
ALTER TABLE tab_account_master ADD CONSTRAINT uq_account_name UNIQUE (company_id, account_name);

-- 2. Address Type Master
ALTER TABLE tab_address_type_master ADD CONSTRAINT uq_address_type_code UNIQUE (company_id, address_type_code);

-- 3. Bank Master
ALTER TABLE tab_bank_master ADD CONSTRAINT uq_bank_account UNIQUE (company_id, ifsc);

-- 4. Branch Master
ALTER TABLE tab_branch_master ADD CONSTRAINT uq_branch_name UNIQUE (company_id, branch_name);

-- 5. Cost Center Master
ALTER TABLE tab_cost_center_master ADD CONSTRAINT uq_cost_center_name UNIQUE (company_id, cost_center_name);

-- 6. Currency Master
ALTER TABLE tab_currency_master ADD CONSTRAINT uq_currency_code UNIQUE (company_id, currency_code);

-- 7. Currency Adjustment
ALTER TABLE tab_currency_adjustment ADD CONSTRAINT uq_currency_adjustment UNIQUE (company_id, currency_id, adjustment_date);

-- 8. Employee
ALTER TABLE tab_employee ADD CONSTRAINT uq_employee_email UNIQUE (company_id, email);

-- 9. Fiscal Year Master
ALTER TABLE tab_fiscal_year_master ADD CONSTRAINT uq_fiscal_year UNIQUE (company_id, start_date, end_date);

-- 10. Godown Master
ALTER TABLE tab_godown_master ADD CONSTRAINT uq_godown_name UNIQUE (company_id, godown_name);

-- 11. HSN Master
ALTER TABLE tab_hsn_master ADD CONSTRAINT uq_hsn_code UNIQUE (company_id, hsn_code);

-- 12. SAC Master
ALTER TABLE tab_sac_master ADD CONSTRAINT uq_sac_code UNIQUE (company_id, sac_code);

-- 13. Unit Master
ALTER TABLE tab_unit_master ADD CONSTRAINT uq_unit_name UNIQUE (company_id, unit_name);

-- 14. Warehouse Master
ALTER TABLE tab_warehouse_master ADD CONSTRAINT uq_warehouse_name UNIQUE (company_id, warehouse_name);

-- 15. Location
ALTER TABLE tab_location ADD CONSTRAINT uq_location_name UNIQUE (company_id, location_name);

-- 16. Ledger Settings
ALTER TABLE tab_ledger_settings ADD CONSTRAINT uq_ledger_settings UNIQUE (company_id);

-- 17. Salesperson Master
ALTER TABLE tab_salesperson_master ADD CONSTRAINT uq_salesperson_email UNIQUE (company_id, email);

-- 18. Project Master
ALTER TABLE tab_project_master ADD CONSTRAINT uq_project_name UNIQUE (company_id, project_name);

-- 19. Payment Mode Master
ALTER TABLE tab_payment_mode_master ADD CONSTRAINT uq_payment_mode UNIQUE (company_id, mode_name);

-- 20. Payment Term Master
ALTER TABLE tab_payment_term_master ADD CONSTRAINT uq_payment_term UNIQUE (company_id, term_name);
