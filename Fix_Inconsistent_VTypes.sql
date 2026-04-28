-- One-time script to fix inconsistent voucher types in tab_jv
-- This ensures existing tax entries show as 'Sales' or 'Purchase' in reports.

UPDATE tab_jv 
SET v_type = 'Sales' 
WHERE (v_type IS NULL OR UPPER(v_type) = 'JV') 
AND description LIKE '% Output - Inv %';

UPDATE tab_jv 
SET v_type = 'Purchase' 
WHERE (v_type IS NULL OR UPPER(v_type) = 'JV') 
AND description LIKE '% Input - Bill %';

-- Force plurals for consistency with newer code
UPDATE tab_jv SET v_type = 'Sales' WHERE UPPER(v_type) = 'SALE';
