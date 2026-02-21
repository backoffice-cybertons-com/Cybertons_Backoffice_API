-- SQL SCRIPT: EMERGENCY CLEANUP & TEST DATA SETUP
-- This script will reset your test tables and create a valid link.

-- 1. Clear existing bad data (TRUNCATE removes all rows)
-- CASCADE will handle the dependencies between Branch and Warehouse automatically
TRUNCATE TABLE tab_branch_master RESTART IDENTITY CASCADE;
TRUNCATE TABLE tab_warehouse_master RESTART IDENTITY CASCADE;

-- 2. Insert a fresh Sample Branch
-- Since we restarted identity, this will get BranchId = 1
INSERT INTO tab_branch_master (company_id, branch_name, address, city, state, country, phone)
VALUES (1, 'Main Head Office', '123 Tech Park', 'Mumbai', 'Maharashtra', 'India', '9876543210');

-- 3. Insert a fresh Linked Warehouse
-- We use BranchId = 1 here because that matches the branch above
INSERT INTO tab_warehouse_master (company_id, warehouse_name, branch_id, address)
VALUES (1, 'Primary Central Warehouse', 1, '456 Industrial Area');

-- VERIFICATION QUERY
SELECT b.branch_id, b.branch_name, w.warehouse_name 
FROM tab_branch_master b
JOIN tab_warehouse_master w ON b.branch_id = w.branch_id;
