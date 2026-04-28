-- =============================================
-- DEEP IDENTITY DUPLICATE VALIDATION
-- =============================================

DROP FUNCTION IF EXISTS sp_party_validate_duplicate(INT, INT, TEXT, TEXT, TEXT, TEXT, TEXT, TEXT[], TEXT[]);
CREATE OR REPLACE FUNCTION sp_party_validate_duplicate(
    p_company_id INT,
    p_party_id INT, -- Pass 0 for new record
    p_display_name TEXT,
    p_gst_no TEXT,
    p_pan TEXT,
    p_email TEXT,
    p_phone_mobile TEXT,
    p_bank_accounts TEXT[], -- Array of account numbers
    p_contact_emails TEXT[] -- Array of contact person emails
)
RETURNS TEXT AS $$
DECLARE
    v_match_found TEXT := NULL;
    v_bank_acc TEXT;
    v_contact_email TEXT;
BEGIN
    -- 1. Check Display Name
    IF EXISTS (
        SELECT 1 FROM tab_party_master 
        WHERE company_id = p_company_id AND party_id != p_party_id 
          AND LOWER(display_name) = LOWER(p_display_name)
    ) THEN
        RETURN 'A party with this Display Name already exists.';
    END IF;

    -- 2. Check GSTIN (if provided)
    IF p_gst_no IS NOT NULL AND p_gst_no != '' THEN
        IF EXISTS (
            SELECT 1 FROM tab_party_master 
            WHERE company_id = p_company_id AND party_id != p_party_id 
              AND LOWER(gst_no) = LOWER(p_gst_no)
        ) THEN
            RETURN 'A party with this GSTIN (' || p_gst_no || ') already exists.';
        END IF;
    END IF;

    -- 3. Check PAN (if provided)
    IF p_pan IS NOT NULL AND p_pan != '' THEN
        IF EXISTS (
            SELECT 1 FROM tab_party_master 
            WHERE company_id = p_company_id AND party_id != p_party_id 
              AND LOWER(pan) = LOWER(p_pan)
        ) THEN
            RETURN 'A party with this PAN (' || p_pan || ') already exists.';
        END IF;
    END IF;

    -- 4. Check Email (if provided)
    IF p_email IS NOT NULL AND p_email != '' THEN
        IF EXISTS (
            SELECT 1 FROM tab_party_master 
            WHERE company_id = p_company_id AND party_id != p_party_id 
              AND LOWER(email) = LOWER(p_email)
        ) THEN
            RETURN 'A party with this Email (' || p_email || ') already exists.';
        END IF;
    END IF;

    -- 5. Check Mobile (if provided)
    IF p_phone_mobile IS NOT NULL AND p_phone_mobile != '' THEN
        IF EXISTS (
            SELECT 1 FROM tab_party_master 
            WHERE company_id = p_company_id AND party_id != p_party_id 
              AND phone_mobile = p_phone_mobile
        ) THEN
            RETURN 'A party with this Mobile Number (' || p_phone_mobile || ') already exists.';
        END IF;
    END IF;

    -- 6. Check Bank Accounts (Deep Check)
    IF p_bank_accounts IS NOT NULL AND array_length(p_bank_accounts, 1) > 0 THEN
        FOREACH v_bank_acc IN ARRAY p_bank_accounts
        LOOP
            IF EXISTS (
                SELECT 1 FROM tab_party_bank_details 
                WHERE company_id = p_company_id AND party_id != p_party_id 
                  AND account_number = v_bank_acc
            ) THEN
                RETURN 'Bank Account Number (' || v_bank_acc || ') is already registered with another party.';
            END IF;
        END LOOP;
    END IF;

    -- 7. Check Contact Person Emails (Deep Check)
    IF p_contact_emails IS NOT NULL AND array_length(p_contact_emails, 1) > 0 THEN
        FOREACH v_contact_email IN ARRAY p_contact_emails
        LOOP
            IF EXISTS (
                SELECT 1 FROM tab_party_contact_persons 
                WHERE company_id = p_company_id AND party_id != p_party_id 
                  AND LOWER(email) = LOWER(v_contact_email)
            ) THEN
                RETURN 'Contact Email (' || v_contact_email || ') is already used by another party.';
            END IF;
        END LOOP;
    END IF;

    RETURN NULL; -- No duplicate found
END;
$$ LANGUAGE plpgsql;
