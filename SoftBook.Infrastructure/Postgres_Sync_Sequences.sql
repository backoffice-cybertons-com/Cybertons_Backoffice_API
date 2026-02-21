-- PostgreSQL Script to Synchronize All Master Table Sequences
-- Run this script if you encounter "duplicate key value violates unique constraint" errors.

DO $$
DECLARE
    r RECORD;
    v_max_id BIGINT;
    v_seq_name TEXT;
BEGIN
    FOR r IN (
        SELECT table_name, column_name 
        FROM information_schema.columns 
        WHERE table_schema = 'public' 
        AND column_default LIKE 'nextval(%'
        AND table_name LIKE 'tab_%'
    ) LOOP
        -- Get the sequence name
        v_seq_name := pg_get_serial_sequence(r.table_name, r.column_name);
        
        IF v_seq_name IS NOT NULL THEN
            -- Get the current max ID
            EXECUTE format('SELECT COALESCE(MAX(%I), 0) FROM %I', r.column_name, r.table_name) INTO v_max_id;
            
            -- Set the sequence to the current max
            PERFORM setval(v_seq_name, GREATEST(v_max_id, 1), v_max_id > 0);
            
            RAISE NOTICE 'Synchronized sequence for table %: Set to %', r.table_name, v_max_id;
        END IF;
    END LOOP;
END $$;
