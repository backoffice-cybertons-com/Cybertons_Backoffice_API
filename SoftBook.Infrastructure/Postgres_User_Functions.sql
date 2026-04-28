-- User Validation
CREATE OR REPLACE FUNCTION sp_validate_user(
    p_username VARCHAR,
    p_password_hash VARCHAR,
    p_company_id INT DEFAULT 1
)
RETURNS SETOF tab_user_master AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tab_user_master
    WHERE (username = p_username OR email = p_username)
      AND TRIM(password_hash) = TRIM(p_password_hash)
      AND (company_id = p_company_id OR p_company_id = 0);
END;
$$ LANGUAGE plpgsql;

-- User Registration
CREATE OR REPLACE FUNCTION sp_user_register(
    p_username VARCHAR,
    p_password_hash VARCHAR,
    p_email VARCHAR,
    p_full_name VARCHAR DEFAULT NULL,
    p_phone VARCHAR DEFAULT NULL,
    p_company_id INT DEFAULT 0
)
RETURNS SETOF tab_user_master AS $$
DECLARE
    v_new_user_id INT;
BEGIN
    -- Check for existing username or email
    IF EXISTS (SELECT 1 FROM tab_user_master WHERE username = p_username) THEN
        RAISE EXCEPTION 'Username already exists';
    END IF;

    IF EXISTS (SELECT 1 FROM tab_user_master WHERE email = p_email) THEN
        RAISE EXCEPTION 'Email already registered';
    END IF;

    -- Insert new user
    INSERT INTO tab_user_master (username, password_hash, email, full_name, phone, is_active, company_id)
    VALUES (p_username, p_password_hash, p_email, p_full_name, p_phone, TRUE, p_company_id)
    RETURNING user_id INTO v_new_user_id;

    -- Return the new user row
    RETURN QUERY SELECT * FROM tab_user_master WHERE user_id = v_new_user_id;
END;
$$ LANGUAGE plpgsql;

-- Get User By Email
CREATE OR REPLACE FUNCTION sp_user_get_by_email(
    p_email VARCHAR
)
RETURNS SETOF tab_user_master AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tab_user_master
    WHERE email = p_email;
END;
$$ LANGUAGE plpgsql;

-- Update Password
CREATE OR REPLACE FUNCTION sp_user_update_password(
    p_email VARCHAR,
    p_new_password_hash VARCHAR
)
RETURNS VOID AS $$
BEGIN
    UPDATE tab_user_master
    SET password_hash = p_new_password_hash
    WHERE email = p_email;
END;
$$ LANGUAGE plpgsql;

-- Sidebar and Permissions
CREATE OR REPLACE FUNCTION usp_get_user_sidebar_and_permissions(
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
    -- 1. Get the role for this user for the specific company (or global if p_company_id = 0)
    SELECT ur.role_id INTO v_role_id
    FROM tab_user_role ur
    JOIN tab_role_master rm ON ur.role_id = rm.role_id
    WHERE ur.user_id = p_user_id
        AND (ur.company_id = p_company_id OR ur.company_id = 0 OR ur.company_id = 1)
        AND rm.is_active = TRUE
        AND COALESCE(rm.is_deleted, FALSE) = FALSE
    LIMIT 1;

    IF v_role_id IS NULL THEN
        RETURN; -- Gracefully return no results if no role is found
    END IF;

    -- 2. Fetch sidebar + permissions, recursively bringing in ALL parent tabs
    RETURN QUERY
    WITH RECURSIVE GrantedTabs AS (
        -- BASE CASE: explicitly granted permissions
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
        
        -- RECURSIVE STEP: progressively find parents of the above
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
