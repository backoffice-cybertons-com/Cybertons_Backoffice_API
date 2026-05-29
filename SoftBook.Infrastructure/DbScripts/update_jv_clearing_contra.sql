-- Update existing incorrect record in tab_party_type_master
UPDATE tab_party_type_master
SET party_type_code = 'JV_CLEARING_CONTRA',
    party_type_name = 'JV Clearing Contra',
    description = 'JV Clearing Contra account',
    report_group = 'JV Clearing Contra'
WHERE party_type_id = 45;

-- Ensure future inserts/updates replace literal 'string' with proper values
CREATE OR REPLACE FUNCTION enforce_jv_clearing_contra()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.party_type_code = 'STRING' THEN
        NEW.party_type_code := 'JV_CLEARING_CONTRA';
    END IF;
    IF LOWER(TRIM(NEW.party_type_name)) = 'string' THEN
        NEW.party_type_name := 'JV Clearing Contra';
    END IF;
    IF LOWER(TRIM(NEW.description)) = 'jv clearing contra account' THEN
        -- keep description as is, just ensure proper case
        NEW.description := 'JV Clearing Contra account';
    END IF;
    IF LOWER(TRIM(NEW.report_group)) = 'string' THEN
        NEW.report_group := 'JV Clearing Contra';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_enforce_jv_clearing_contra ON tab_party_type_master;
CREATE TRIGGER trg_enforce_jv_clearing_contra
BEFORE INSERT OR UPDATE ON tab_party_type_master
FOR EACH ROW EXECUTE FUNCTION enforce_jv_clearing_contra();
