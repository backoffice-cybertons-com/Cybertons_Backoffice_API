-----------------------------------------------------------
-- SoftAccDb Row Level Security (RLS) Setup
-- Enforces tenant isolation via company_id for ALL tables
-- that have a "company_id" column in schema "public".
--
-- How it works:
-- - Your application sets a session variable: app.company_id
-- - RLS policy uses it to filter rows automatically
--
-- IMPORTANT:
-- - Run this AFTER creating all tables in SoftAccDb.
-- - You must set app.company_id on every DB connection.
--
-- Example (per connection):
--   SET app.company_id = '123';
--   -- now all queries auto-filter to company_id=123
-----------------------------------------------------------

-- Optional: Make missing app.company_id block access (recommended)
-- If app.company_id is not set, current_setting(..., true) returns NULL
-- and the policy will evaluate to FALSE (no rows returned).

DO $$
DECLARE
  r RECORD;
  v_table TEXT;
  v_policy_name TEXT := 'company_isolation';
BEGIN
  -- Loop all user tables in public schema that contain company_id column
  FOR r IN
    SELECT
      c.table_schema,
      c.table_name
    FROM information_schema.columns c
    JOIN information_schema.tables t
      ON t.table_schema = c.table_schema
     AND t.table_name = c.table_name
    WHERE c.table_schema = 'public'
      AND t.table_type = 'BASE TABLE'
      AND c.column_name = 'company_id'
    ORDER BY c.table_name
  LOOP
    v_table := format('%I.%I', r.table_schema, r.table_name);

    -- Enable RLS
    EXECUTE format('ALTER TABLE %s ENABLE ROW LEVEL SECURITY', v_table);

    -- Optional: force RLS even for table owner (recommended for safety)
    -- If you use a superuser role in production, superusers bypass RLS.
    EXECUTE format('ALTER TABLE %s FORCE ROW LEVEL SECURITY', v_table);

    -- Drop existing policy (idempotent)
    EXECUTE format('DROP POLICY IF EXISTS %I ON %s', v_policy_name, v_table);

    -- Create policy: only rows for current company_id
    EXECUTE format(
      'CREATE POLICY %I ON %s USING (company_id = current_setting(''app.company_id'', true)::int)',
      v_policy_name, v_table
    );
  END LOOP;
END $$;



