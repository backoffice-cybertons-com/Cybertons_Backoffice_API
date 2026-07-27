-- Insert 'GST REPORT FOR CA' tab under the 'GST REPORT' parent tab
-- This assumes 'GST REPORT' exists in the tabs table and the roles need permissions

DO $$ 
DECLARE
    gst_report_parent_id INT;
    new_tab_id INT;
    r RECORD;
BEGIN
    -- 1. Find the parent tab_id for 'GST REPORT'
    SELECT tab_id INTO gst_report_parent_id FROM tabs WHERE tab_name ILIKE '%GST REPORT%' AND parent_id IS NOT NULL LIMIT 1;

    -- If there's no dedicated GST REPORT tab but there's a Reports tab, fallback to Reports
    IF gst_report_parent_id IS NULL THEN
        SELECT tab_id INTO gst_report_parent_id FROM tabs WHERE tab_name ILIKE 'Reports' LIMIT 1;
    END IF;

    -- 2. Insert the new tab if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM tabs WHERE tab_name = 'GST REPORT FOR CA') THEN
        INSERT INTO tabs (tab_name, tab_url, parent_id, icon_path)
        VALUES ('GST REPORT FOR CA', '/reports/gst-report-ca', gst_report_parent_id, 'fa-file-excel')
        RETURNING tab_id INTO new_tab_id;

        -- 3. Grant permission to all Roles
        FOR r IN (SELECT DISTINCT role_id FROM tab_role_permission) LOOP
            INSERT INTO tab_role_permission (role_id, tab_id, can_view, can_add, can_edit, can_delete, is_active)
            VALUES (r.role_id, new_tab_id, TRUE, TRUE, TRUE, TRUE, TRUE)
            ON CONFLICT DO NOTHING;
        END LOOP;
        
        RAISE NOTICE 'Added GST REPORT FOR CA with tab_id %', new_tab_id;
    ELSE
        RAISE NOTICE 'GST REPORT FOR CA tab already exists.';
    END IF;
END $$;
