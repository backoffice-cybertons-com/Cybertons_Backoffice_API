-- SCRIPT TO UPDATE COMPANY CONNECTION STRINGS IN THE RESTORED DATABASE
-- Run this in your 'softaccdb_1_1304' database using pgAdmin or any SQL tool.

-- Step 1: List current connection strings (for reference)
SELECT company_id, company_name, connection_string 
FROM tab_company_master;

-- Step 2: Update all companies to point to the local server
-- This replaces the host and database names if they was previously pointing to 'live'
-- Note: Adjust the database name in the connection string if you have separate DBs for each company.
-- If all data is now in 'softaccdb_1_1304', you should point them there.

UPDATE tab_company_master
SET connection_string = 'Host=192.168.1.207;Database=softaccdb_1_1304;Username=postgres;Password=Soft123'
WHERE company_id = 1; -- Update for each company ID as needed

-- If you have multiple companies and they all share the same database:
-- UPDATE tab_company_master
-- SET connection_string = 'Host=192.168.1.207;Database=softaccdb_1_1304;Username=postgres;Password=Soft123';

-- Step 3: Verify the update
SELECT company_id, company_name, connection_string 
FROM tab_company_master;
