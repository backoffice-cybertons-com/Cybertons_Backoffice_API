-- Sidebar permissions function used by UserRepository.GetSidebarPermissionsAsync (two parameters).
-- If a database still has the legacy single-argument overload only, the API fails with:
--   function usp_get_user_sidebar_and_permissions(p_user_id => integer, p_company_id => integer) does not exist
-- Run this script on that tenant database (after backup).

DROP FUNCTION IF EXISTS public.usp_get_user_sidebar_and_permissions(integer);

CREATE OR REPLACE FUNCTION public.usp_get_user_sidebar_and_permissions(
    p_user_id INT,
    p_company_id INT
)
RETURNS TABLE (
    role_id INT,
    role_name VARCHAR,
    tab_id INT,
    tab_name VARCHAR,
    tab_url VARCHAR,
    parent_id INT,
    icon_path VARCHAR,
    sort_order INT,
    can_view BOOLEAN,
    can_add BOOLEAN,
    can_edit BOOLEAN,
    can_delete BOOLEAN
) AS $$
DECLARE
    v_role_id INT;
BEGIN
    SELECT ur.role_id INTO v_role_id
    FROM tab_user_role ur
    JOIN tab_role_master rm ON ur.role_id = rm.role_id
    WHERE ur.user_id = p_user_id
        AND (ur.company_id = p_company_id OR ur.company_id = 0 OR ur.company_id = 1)
        AND rm.is_active = TRUE
        AND COALESCE(rm.is_deleted, FALSE) = FALSE
    LIMIT 1;

    IF v_role_id IS NULL THEN
        RETURN;
    END IF;

    RETURN QUERY
    WITH RECURSIVE GrantedTabs AS (
        SELECT  
            p.role_id,
            t.tab_id,
            t.parent_id,
            p.can_view,
            p.can_add,
            p.can_edit,
            p.can_delete
        FROM tab_role_permission p
        JOIN tabs t ON p.tab_id = t.tab_id
        WHERE 
            p.role_id = v_role_id
            AND (p.company_id = p_company_id OR p.company_id = 0 OR p.company_id = 1)
            AND p.can_view = TRUE
            AND p.is_active = TRUE
            AND t.is_active = TRUE
            
        UNION
        
        SELECT 
            gt.role_id,
            parent.tab_id,
            parent.parent_id,
            TRUE AS can_view,
            FALSE AS can_add,
            FALSE AS can_edit,
            FALSE AS can_delete
        FROM GrantedTabs gt
        JOIN tabs parent ON gt.parent_id = parent.tab_id
        WHERE parent.is_active = TRUE
    )
    SELECT 
        rm.role_id,
        rm.role_name,
        t.tab_id,
        t.tab_name,
        t.tab_url,
        CASE WHEN t.parent_id = 0 THEN NULL ELSE t.parent_id END as parent_id,
        t.icon_path,
        t.sort_order,
        BOOL_OR(gt.can_view) as can_view,
        BOOL_OR(gt.can_add) as can_add,
        BOOL_OR(gt.can_edit) as can_edit,
        BOOL_OR(gt.can_delete) as can_delete
    FROM GrantedTabs gt
    JOIN tabs t ON gt.tab_id = t.tab_id
    JOIN tab_role_master rm ON rm.role_id = gt.role_id
    WHERE rm.is_active = TRUE AND COALESCE(rm.is_deleted, FALSE) = FALSE
    GROUP BY 
        rm.role_id,
        rm.role_name,
        t.tab_id,
        t.tab_name,
        t.tab_url,
        t.parent_id,
        t.icon_path,
        t.sort_order
    ORDER BY 
        t.parent_id,
        t.sort_order,
        t.tab_name;
END;
$$ LANGUAGE plpgsql;
