-- MASS UPDATE SCRIPT: Align Company ID 1 to 10001
-- Run this in your 'softaccdb_1_1304' database using pgAdmin.

DO $$
DECLARE
    r RECORD;
    v_old_id INT := 1;
    v_new_id INT := 10001;
    v_sql TEXT;
BEGIN
    -- Loop through all tables in the public schema that have a company_id column
    FOR r IN (
        SELECT table_name 
        FROM information_schema.columns 
        WHERE table_schema = 'public' 
          AND column_name = 'company_id'
          AND table_name NOT LIKE 'tab_company_master' -- Skip master table if it exists here
    ) LOOP
        -- Generate and execute dynamic SQL to update the company_id with error handling
        v_sql := format('UPDATE %I SET company_id = %L WHERE company_id = %L', r.table_name, v_new_id, v_old_id);
        
        BEGIN
            EXECUTE v_sql;
            -- RAISE NOTICE 'Updated table: %', r.table_name;
        EXCEPTION WHEN unique_violation THEN
            RAISE NOTICE 'Skipped table % due to unique constraint (records for ID % already exist)', r.table_name, v_new_id;
        WHEN OTHERS THEN
            RAISE NOTICE 'Error updating table %: %', r.table_name, SQLERRM;
        END;
    END LOOP;
    
    RAISE NOTICE 'Mass update complete. All records shifted from ID % to ID %', v_old_id, v_new_id;
END $$;

-- VERIFICATION QUERIES
SELECT 'tab_party_master' as table_name, company_id, count(*) 
FROM tab_party_master 
GROUP BY company_id;

-- Add other important tables here to verify
SELECT 'tab_purchase_order' as table_name, company_id, count(*) 
FROM tab_purchase_order 
GROUP BY company_id;
