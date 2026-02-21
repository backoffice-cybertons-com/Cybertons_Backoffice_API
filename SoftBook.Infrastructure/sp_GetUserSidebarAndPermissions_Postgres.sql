-- FUNCTION: public.usp_get_user_sidebar_and_permissions(integer)

-- DROP FUNCTION IF EXISTS public.usp_get_user_sidebar_and_permissions(integer);

CREATE OR REPLACE FUNCTION public.usp_get_user_sidebar_and_permissions(
	p_user_id integer)
    RETURNS TABLE(role_id integer, role_name character varying, tab_id integer, tab_name character varying, tab_url character varying, parent_id integer, icon_path character varying, sort_order integer, can_view boolean, can_add boolean, can_edit boolean, can_delete boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    v_role_id INT;
BEGIN
    -- 1. Get the role for this user
    SELECT ur.role_id INTO v_role_id
    FROM tab_user_role ur
    JOIN tab_role_master rm ON ur.role_id = rm.role_id
    WHERE ur.user_id = p_user_id
        AND rm.is_active = TRUE
        AND rm.is_deleted = FALSE
    LIMIT 1;

    IF v_role_id IS NULL THEN
        RAISE EXCEPTION 'No active role assigned to this user.';
    END IF;

    -- 2. Fetch sidebar + permissions
    RETURN QUERY
    SELECT  
        rm.role_id,
        rm.role_name,
        t.tab_id,
        t.tab_name,
        t.tab_url,
        t.parent_id,
        t.icon_path,
        t.sort_order,
        p.can_view,
        p.can_add,
        p.can_edit,
        p.can_delete
    FROM tab_role_permission p
    JOIN tabs t ON p.tab_id = t.tab_id
    JOIN tab_role_master rm ON p.role_id = rm.role_id
    WHERE 
        p.role_id = v_role_id
        AND p.can_view = TRUE
        AND p.is_active = TRUE
        AND t.is_active = TRUE
        AND rm.is_active = TRUE
        AND rm.is_deleted = FALSE
    ORDER BY 
        t.parent_id,
        t.sort_order,
        t.tab_name;
END;
$BODY$;

ALTER FUNCTION public.usp_get_user_sidebar_and_permissions(integer)
    OWNER TO postgres;
