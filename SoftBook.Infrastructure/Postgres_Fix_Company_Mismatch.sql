-- =============================================
-- FIX COMPANY ID MISMATCH
-- Standardize all data to Company 1
-- =============================================

-- 1. Update all users to Company 1
UPDATE tab_user_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;

-- 2. Update user roles to Company 1
UPDATE tab_user_role SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;

-- 3. Ensure all parties are Company 1 (already seem to be, but for safety)
UPDATE tab_party_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;

-- 4. Ensure other masters are Company 1
UPDATE tab_branch_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;
UPDATE tab_warehouse_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;
UPDATE tab_account_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;
UPDATE tab_product_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;
UPDATE tab_unit_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;
UPDATE tab_tax_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;
UPDATE tab_hsn_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;
UPDATE tab_sac_master SET company_id = 1 WHERE company_id IS NULL OR company_id != 1;

-- 5. Ensure the company exists in companymaster (Run this in companymaster DB)
-- INSERT INTO tab_company_master (company_id, company_name, connection_string, is_enabled)
-- VALUES (1, 'SoftBook Demo Company', 'Host=localhost;Database=softaccdb_1;Username=postgres;Password=Soft123', TRUE)
-- ON CONFLICT (company_id) DO NOTHING;
