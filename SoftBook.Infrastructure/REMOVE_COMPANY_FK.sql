-----------------------------------------------------------
-- SCRIPT TO REMOVE FOREIGN KEY CONSTRAINTS TO tab_company_master
-- 
-- IMPORTANT: tab_company_master is in a DIFFERENT database (CompanyMaster)
-- PostgreSQL does not support cross-database foreign keys
-- Therefore, these constraints must be removed
--
-- Run this script AFTER creating tables if you want to remove the FKs
-----------------------------------------------------------

-- Note: This script removes all foreign key constraints referencing tab_company_master
-- Since tab_company_master is in CompanyMaster database (separate),
-- these constraints cannot exist in SoftAccDb

-- Example removal (you'll need to run similar for each table):
-- ALTER TABLE tab_account_master DROP CONSTRAINT IF EXISTS fk_account_company;
-- ALTER TABLE tab_address_type_master DROP CONSTRAINT IF EXISTS fk_address_type_company;
-- ... etc

-- OR: Simply remove the CONSTRAINT lines from SoftAccDB_PostgreSQL.sql before running
-- The tables will work fine without these FKs - application code validates CompanyId

-- RECOMMENDED: Keep CompanyId column and indexes, but remove FK constraints
-- Use Row-Level Security (RLS) for automatic filtering instead


