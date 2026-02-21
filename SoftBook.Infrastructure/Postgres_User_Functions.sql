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
    p_phone VARCHAR DEFAULT NULL
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
    VALUES (p_username, p_password_hash, p_email, p_full_name, p_phone, TRUE, 0) -- Assuming company_id 0 or handled by context
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
    p_user_id INT
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
$$ LANGUAGE plpgsql;
