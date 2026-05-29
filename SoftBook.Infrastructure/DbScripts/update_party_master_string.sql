-- 1. Fix the existing 'string' record in tab_party_master
UPDATE tab_party_master
SET display_name = 'JV Clearing Contra',
    company_name = 'JV Clearing Contra'
WHERE TRIM(LOWER(display_name)) = 'string' OR TRIM(LOWER(company_name)) = 'string';

-- 2. Add a trigger to tab_party_master to auto-correct 'string' on new inserts/updates
CREATE OR REPLACE FUNCTION enforce_party_master_string()
RETURNS TRIGGER AS $$
BEGIN
    -- If the incoming display_name is 'string', change it
    IF LOWER(TRIM(NEW.display_name)) = 'string' THEN
        NEW.display_name := 'JV Clearing Contra';
    END IF;
    
    -- If the incoming company_name is 'string', change it
    IF LOWER(TRIM(NEW.company_name)) = 'string' THEN
        NEW.company_name := 'JV Clearing Contra';
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Drop any existing trigger
DROP TRIGGER IF EXISTS trg_enforce_party_master_string ON tab_party_master;

-- Attach the trigger to tab_party_master
CREATE TRIGGER trg_enforce_party_master_string
BEFORE INSERT OR UPDATE ON tab_party_master
FOR EACH ROW EXECUTE FUNCTION enforce_party_master_string();
