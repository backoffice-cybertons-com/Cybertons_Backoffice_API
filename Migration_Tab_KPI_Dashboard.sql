-- ================================================================
-- KPI Dashboard sidebar menu (PostgreSQL)
-- Adds "KPI Dashboard" below Dashboard/Home in the sidebar
-- Run on your live PostgreSQL database (pgAdmin / psql)
-- ================================================================

-- STEP 1: Find the Dashboard/Home parent tab
SELECT tab_id, tab_name, tab_url, parent_id, sort_order
FROM tabs
WHERE tab_name ILIKE '%dashboard%' OR tab_name = 'Home'
ORDER BY tab_id;

-- STEP 2: Insert KPI Dashboard as child under Dashboard/Home (tab_id = 1)
-- Adjust parent_id if your Dashboard tab uses a different tab_id
INSERT INTO tabs (tab_id, tab_name, tab_url, parent_id, icon_path, sort_order, is_active, company_id)
SELECT
    1015,
    'KPI Dashboard',
    '/dashboard/kpi',
    1,
    'fa-solid fa-chart-pie',
    2,
    true,
    1
WHERE NOT EXISTS (
    SELECT 1 FROM tabs WHERE tab_id = 1015
);

-- If row already exists, keep URL and label in sync
UPDATE tabs
SET
    tab_name = 'KPI Dashboard',
    tab_url = '/dashboard/kpi',
    parent_id = 1,
    icon_path = 'fa-solid fa-chart-pie',
    sort_order = 2,
    is_active = true
WHERE tab_id = 1015;

-- Optional: add "Overview" child for main dashboard (skip if you already have it)
INSERT INTO tabs (tab_id, tab_name, tab_url, parent_id, icon_path, sort_order, is_active, company_id)
SELECT
    1016,
    'Overview',
    '/dashboard',
    1,
    'fa-solid fa-gauge-high',
    1,
    true,
    1
WHERE NOT EXISTS (
    SELECT 1 FROM tabs WHERE tab_id = 1016
);

-- Optional: make Home/Dashboard a parent menu (no direct link)
UPDATE tabs
SET tab_url = '/#'
WHERE tab_id = 1
  AND tab_name IN ('Home', 'Dashboard')
  AND EXISTS (SELECT 1 FROM tabs WHERE parent_id = 1 AND tab_id IN (1015, 1016));

-- STEP 3: Grant permissions — copy from roles that can view Dashboard (tab_id = 1)
INSERT INTO tab_role_permission (company_id, role_id, tab_id, can_view, can_add, can_edit, can_delete, is_active)
SELECT
    p.company_id,
    p.role_id,
    1015,
    p.can_view,
    false,
    false,
    false,
    true
FROM tab_role_permission p
WHERE p.tab_id = 1
  AND p.can_view = true
  AND p.is_active = true
  AND NOT EXISTS (
      SELECT 1 FROM tab_role_permission x
      WHERE x.tab_id = 1015 AND x.role_id = p.role_id AND x.company_id = p.company_id
  );

INSERT INTO tab_role_permission (company_id, role_id, tab_id, can_view, can_add, can_edit, can_delete, is_active)
SELECT
    p.company_id,
    p.role_id,
    1016,
    p.can_view,
    false,
    false,
    false,
    true
FROM tab_role_permission p
WHERE p.tab_id = 1
  AND p.can_view = true
  AND p.is_active = true
  AND NOT EXISTS (
      SELECT 1 FROM tab_role_permission x
      WHERE x.tab_id = 1016 AND x.role_id = p.role_id AND x.company_id = p.company_id
  );

-- STEP 4: Verify
SELECT t.tab_id, t.tab_name, t.tab_url, t.parent_id, t.sort_order, t.is_active,
       p.role_id, p.can_view
FROM tabs t
LEFT JOIN tab_role_permission p ON t.tab_id = p.tab_id AND p.is_active = true
WHERE t.tab_id IN (1, 1015, 1016)
ORDER BY t.parent_id NULLS FIRST, t.sort_order, t.tab_id, p.role_id;

-- ================================================================
-- SQL Server variant (if you use SoftAccDb on SQL Server)
-- ================================================================
/*
DECLARE @DashboardTabId INT = 1;

IF NOT EXISTS (SELECT 1 FROM Tabs WHERE TabId = 1015)
BEGIN
    SET IDENTITY_INSERT Tabs ON;
    INSERT INTO Tabs (TabId, TabName, TabUrl, ParentId, IconPath, SortOrder, IsActive)
    VALUES (1015, N'KPI Dashboard', N'/dashboard/kpi', @DashboardTabId, N'fa-solid fa-chart-pie', 2, 1);
    SET IDENTITY_INSERT Tabs OFF;
END
ELSE
BEGIN
    UPDATE Tabs
    SET TabName = N'KPI Dashboard', TabUrl = N'/dashboard/kpi', ParentId = @DashboardTabId,
        IconPath = N'fa-solid fa-chart-pie', SortOrder = 2, IsActive = 1
    WHERE TabId = 1015;
END

INSERT INTO TabRolePermission (RoleId, TabId, CanView, CanAdd, CanEdit, CanDelete, IsActive)
SELECT p.RoleId, 1015, p.CanView, 0, 0, 0, 1
FROM TabRolePermission p
WHERE p.TabId = @DashboardTabId AND p.CanView = 1 AND p.IsActive = 1
  AND NOT EXISTS (SELECT 1 FROM TabRolePermission x WHERE x.TabId = 1015 AND x.RoleId = p.RoleId);
*/
