-- Migration to add total_quantity to transaction headers
ALTER TABLE tab_sale_pur ADD COLUMN IF NOT EXISTS total_quantity NUMERIC(18,2) DEFAULT 0;

-- Update existing records with calculated totals from line items
UPDATE tab_sale_pur h
SET total_quantity = COALESCE((
    SELECT SUM(quantity)
    FROM tab_sal_pur_tran l
    WHERE l.sp_unique_code = h.sp_unique_code
    AND (l.delete_flag = FALSE OR l.delete_flag IS NULL)
), 0);
