-- ================================================================
-- LIVE SERVER DEPLOYMENT SCRIPT
-- Date: 06 April 2026
-- Feature: Income/Expense Direct Journal Module
-- Run this script on pgAdmin or any PostgreSQL client
-- ================================================================

-- ----------------------------------------------------------------
-- STEP 1: Check if tab_id = 1014 for Income/Expense already exists
-- ----------------------------------------------------------------
SELECT tab_id, tab_name, sort_order FROM tabs WHERE tab_id IN (23, 1014);

-- ----------------------------------------------------------------
-- STEP 2: Rename Payment/Receipt tab from old name to 'Pay/Rec'
-- (only if it exists, safe to run multiple times)
-- ----------------------------------------------------------------
UPDATE tabs 
SET tab_name = 'Pay/Rec' 
WHERE tab_id = 23;

-- ----------------------------------------------------------------
-- STEP 3: Insert the new Income/Expense tab if it does not exist
-- parent_id = 4 means it goes under "Purchases" in the sidebar menu
-- ----------------------------------------------------------------
INSERT INTO tabs (tab_id, tab_name, tab_url, parent_id, icon_path, sort_order, is_active, company_id)
SELECT 1014, 'Income/Expense', '/purchases/expense', 4, 'fa-solid fa-money-bill-trend-up', 9, true, 1
WHERE NOT EXISTS (
    SELECT 1 FROM tabs WHERE tab_id = 1014
);

-- If it already exists, just update the label:
UPDATE tabs
SET tab_name = 'Income/Expense', tab_url = '/purchases/expense'
WHERE tab_id = 1014;

-- ----------------------------------------------------------------
-- STEP 4: Grant permissions to ALL 3 roles that have access to tab 23
-- (role_id 1, 4, 9 - copied from your live server tab_role_permission)
-- ----------------------------------------------------------------

-- Role 1 (Admin - full access)
INSERT INTO tab_role_permission (company_id, role_id, tab_id, can_view, can_add, can_edit, can_delete, is_active)
SELECT 1, 1, 1014, true, true, true, true, true
WHERE NOT EXISTS (
    SELECT 1 FROM tab_role_permission WHERE tab_id = 1014 AND role_id = 1 AND company_id = 1
);

-- Role 4
INSERT INTO tab_role_permission (company_id, role_id, tab_id, can_view, can_add, can_edit, can_delete, is_active)
SELECT 1, 4, 1014, true, false, false, false, true
WHERE NOT EXISTS (
    SELECT 1 FROM tab_role_permission WHERE tab_id = 1014 AND role_id = 4 AND company_id = 1
);

-- Role 9
INSERT INTO tab_role_permission (company_id, role_id, tab_id, can_view, can_add, can_edit, can_delete, is_active)
SELECT 1, 9, 1014, true, true, true, true, true
WHERE NOT EXISTS (
    SELECT 1 FROM tab_role_permission WHERE tab_id = 1014 AND role_id = 9 AND company_id = 1
);

-- ----------------------------------------------------------------
-- STEP 5: Verify - view what was created
-- ----------------------------------------------------------------
SELECT t.tab_id, t.tab_name, t.tab_path, t.sort_order,
       p.role_id, p.can_view, p.can_add, p.can_edit, p.can_delete
FROM tabs t
LEFT JOIN tab_role_permission p ON t.tab_id = p.tab_id
WHERE t.tab_id IN (23, 1014)
ORDER BY t.tab_id;

-- ================================================================
-- NOTE: No structural (DDL) changes to the database are needed.
-- The Income/Expense entries are saved in the existing tab_jv table.
-- GCode format example saved: JV-20260406-175044
-- v_type saved as 'Income' or 'Expense'
-- ================================================================
