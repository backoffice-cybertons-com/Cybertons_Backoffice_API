-- Function to handle bulk saving of role permissions
-- This ensures that setting permissions for a role is atomic and efficient.

CREATE OR REPLACE FUNCTION fn_role_permission_bulk_save(
    p_company_id INT,
    p_role_id INT,
    p_permissions JSONB
) RETURNS VOID AS $$
BEGIN
    -- Delete existing permissions for this role and company to perform a full synchronization
    DELETE FROM tab_role_permission 
    WHERE company_id = p_company_id AND role_id = p_role_id;

    -- Insert updated permissions from the provided JSONB array
    INSERT INTO tab_role_permission (
        company_id, 
        role_id, 
        tab_id, 
        can_view, 
        can_add, 
        can_edit, 
        can_delete, 
        is_active
    )
    SELECT 
        p_company_id,
        p_role_id,
        COALESCE((elem->>'tabId'), (elem->>'TabId'))::INT,
        COALESCE((elem->>'canView'), (elem->>'CanView'))::BOOLEAN,
        COALESCE((elem->>'canAdd'), (elem->>'CanAdd'))::BOOLEAN,
        COALESCE((elem->>'canEdit'), (elem->>'CanEdit'))::BOOLEAN,
        COALESCE((elem->>'canDelete'), (elem->>'CanDelete'))::BOOLEAN,
        TRUE
    FROM jsonb_array_elements(p_permissions) AS elem;

    RAISE NOTICE 'Permissions for Role ID % successfully updated.', p_role_id;
END;
$$ LANGUAGE plpgsql;
