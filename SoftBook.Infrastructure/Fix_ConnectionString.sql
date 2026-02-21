-- FIX CONNECTION STRING IN COMPANYMASTER DATABASE
-- Run this in 'companymaster' database using pgAdmin 4

-- Step 1: Check current connection string
SELECT company_id, company_name, connection_string 
FROM tab_company_master 
WHERE company_id = 1;

-- Step 2: Update to correct PostgreSQL connection string
UPDATE tab_company_master
SET connection_string = 'Host=localhost;Database=softaccdb_1;Username=postgres;Password=Soft123'
WHERE company_id = 1;

-- Step 3: Verify the update
SELECT company_id, company_name, connection_string 
FROM tab_company_master 
WHERE company_id = 1;

/* 
The connection string MUST be PostgreSQL format:
- Host=localhost
- Database=softaccdb_1
- Username=postgres
- Password=Soft123

DO NOT include SQL Server parameters like:
- Server=
- MultipleActiveResultSets=
- TrustServerCertificate=
- User Id= (use Username= instead)
*/
