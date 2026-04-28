-- 1. Rename the old payment receipt tab correctly
UPDATE tabs 
SET tab_name = 'Pay/Rec' 
WHERE tab_id = 23;

-- 2. Ensure the new Expense tab has a valid sort order
UPDATE tabs 
SET sort_order = 3, tab_name = 'Expense' 
WHERE tab_id = 1014;

-- 3. Just in case permissions failed before, grant permissions for the new tab (1014)
INSERT INTO tab_role_permission (company_id, role_id, tab_id, can_view, can_add, can_edit, can_delete, is_active)
SELECT 1, 1, 1014, true, true, true, true, true
WHERE NOT EXISTS (
    SELECT 1 FROM tab_role_permission WHERE tab_id = 1014 AND role_id = 1 AND company_id = 1
);
