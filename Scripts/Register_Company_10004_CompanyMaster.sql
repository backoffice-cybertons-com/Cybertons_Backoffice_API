-- =========================================================
-- STEP 1 ONLY — Register company in CompanyMaster
-- Database: companymaster
-- (appsettings.json → ConnectionStrings:DefaultConnection)
-- =========================================================
-- After this, seed tenant DB softaccdb_1_3107 using:
--   Scripts/Register_Company_10004_Tenant.sql
-- Full guide:
--   Docs/Register_New_Company_Guide.md
-- =========================================================

INSERT INTO tab_company_master (
    company_id,
    company_name,
    connection_string,
    is_enabled,
    priority
)
VALUES (
    10004,
    'Hetak Demo Co 10004',   -- <<< change display name as needed
    'Host=192.168.1.207;Database=softaccdb_1_3107;Username=postgres;Password=Soft123',
    TRUE,
    0
)
ON CONFLICT (company_id) DO UPDATE
SET
    company_name      = EXCLUDED.company_name,
    connection_string = EXCLUDED.connection_string,
    is_enabled        = TRUE;

-- Verify routing
SELECT company_id, company_name, connection_string, is_enabled, priority, created_at
FROM tab_company_master
WHERE company_id = 10004;
