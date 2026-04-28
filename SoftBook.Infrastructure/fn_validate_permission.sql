-- Centralized Permission Validation Function
-- Validates if a user has ADD, EDIT, or DELETE rights for a specific module (tab).
-- Returns TRUE if allowed, raises EXCEPTION if denied.

CREATE OR REPLACE FUNCTION public.fn_validate_permission(
    p_user_id INT,
    p_company_id INT,
    p_tab_id INT,
    p_action VARCHAR(20) -- 'ADD', 'EDIT', 'DELETE'
) 
RETURNS BOOLEAN AS $body$
DECLARE
    v_has_permission BOOLEAN := FALSE;
    v_role_id INT;
BEGIN
    -- 1. If user_id is NULL, we can't validate. Assume denied for safety in production.
    -- (In development, you might want to return TRUE if p_user_id is null, but FOR SECURITY we block)
    IF p_user_id IS NULL OR p_user_id = 0 THEN
        RAISE EXCEPTION 'Security Error: User ID is required for permission validation.';
    END IF;

    -- 2. Get User's Role (Assumes 1 role per user per company for simplicity, or checks active one)
    SELECT role_id INTO v_role_id 
    FROM public.tab_user_role 
    WHERE user_id = p_user_id AND company_id = p_company_id AND is_active = 1
    LIMIT 1;

    IF v_role_id IS NULL THEN
        RAISE EXCEPTION 'Security Error: No active role found for user % in company %.', p_user_id, p_company_id;
    END IF;

    -- 3. Check specific permission
    SELECT 
        CASE 
            WHEN UPPER(p_action) = 'ADD'    THEN can_add = 1
            WHEN UPPER(p_action) = 'EDIT'   THEN can_edit = 1
            WHEN UPPER(p_action) = 'DELETE' THEN can_delete = 1
            ELSE FALSE
        END INTO v_has_permission
    FROM public.tab_role_permission
    WHERE role_id = v_role_id 
      AND tab_id = p_tab_id 
      AND company_id = p_company_id
      AND is_active = 1;

    IF v_has_permission IS NULL OR v_has_permission = FALSE THEN
        RAISE EXCEPTION 'Access Denied: You do not have % permission for this module (Tab ID: %).', UPPER(p_action), p_tab_id;
    END IF;

    RETURN TRUE;
END;
$body$ LANGUAGE plpgsql;
