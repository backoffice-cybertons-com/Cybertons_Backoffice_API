-- MIGRATION SCRIPT: Rename Cost Price to COGS in Transaction Table
-- Target: PostgreSQL

-- 1. RENAME COLUMN
ALTER TABLE tab_sal_pur_tran RENAME COLUMN cost_price TO cogs;

-- 2. DROP AND RE-CREATE fn_sal_pur_tran (Line Items)
-- Note: Required because parameter names cannot be changed via CREATE OR REPLACE FUNCTION.
DROP FUNCTION IF EXISTS fn_sal_pur_tran(
    VARCHAR, INT, INT, INT, INT, TEXT, DATE, VARCHAR, NUMERIC, INT, NUMERIC, INT, 
    NUMERIC, NUMERIC, NUMERIC, NUMERIC, NUMERIC, NUMERIC, NUMERIC, NUMERIC, NUMERIC, 
    NUMERIC, NUMERIC, NUMERIC, VARCHAR, VARCHAR, INT, INT, INT, INT, INT, BOOLEAN, NUMERIC, NUMERIC
);

CREATE OR REPLACE FUNCTION fn_sal_pur_tran(
    p_action VARCHAR,
    p_sp_tran_unique_code INT DEFAULT NULL,
    p_sp_unique_code INT DEFAULT NULL,
    p_company_id INT DEFAULT NULL,
    p_product_id INT DEFAULT NULL,
    p_product_details TEXT DEFAULT NULL,
    p_batch_date DATE DEFAULT NULL,
    p_batch_no VARCHAR DEFAULT NULL,
    p_mrp NUMERIC DEFAULT NULL,
    p_sub_unit INT DEFAULT NULL,
    p_sub_unit_quantity NUMERIC DEFAULT NULL,
    p_unit INT DEFAULT NULL,
    p_quantity NUMERIC DEFAULT NULL,
    p_rate NUMERIC DEFAULT NULL,
    p_discount_per NUMERIC DEFAULT 0,
    p_amount NUMERIC DEFAULT NULL,
    p_igst_percent NUMERIC DEFAULT NULL,
    p_igst_amount NUMERIC DEFAULT NULL,
    p_cgst_percent NUMERIC DEFAULT NULL,
    p_cgst_amount NUMERIC DEFAULT NULL,
    p_sgst_percent NUMERIC DEFAULT NULL,
    p_sgst_amount NUMERIC DEFAULT NULL,
    p_total_amount NUMERIC DEFAULT NULL,
    p_crt NUMERIC DEFAULT NULL,
    p_packing VARCHAR DEFAULT NULL,
    p_hsn_code VARCHAR DEFAULT NULL,
    p_fiscal_year_id INT DEFAULT NULL,
    p_branch_id INT DEFAULT NULL,
    p_created_by INT DEFAULT NULL,
    p_modified_by INT DEFAULT NULL,
    p_deleted_by INT DEFAULT NULL,
    p_delete_flag BOOLEAN DEFAULT FALSE,
    p_rate_diff NUMERIC DEFAULT 0,
    p_cogs NUMERIC DEFAULT 0
)
RETURNS SETOF tab_sal_pur_tran AS $$
BEGIN
    IF p_action = 'INSERT' THEN
        RETURN QUERY
        INSERT INTO tab_sal_pur_tran (
            sp_unique_code, company_id, product_id, product_details, batch_date, batch_no, mrp, 
            sub_unit, sub_unit_quantity, unit, quantity, rate, discount_per, amount,
            igst_percent, igst_amount, cgst_percent, cgst_amount, sgst_percent, sgst_amount,
            total_amount, crt, packing, hsn_code, fiscal_year_id, branch_id, created_by, created_on, 
            delete_flag, rate_diff, cogs
        )
        VALUES (
            p_sp_unique_code, p_company_id, p_product_id, p_product_details, p_batch_date, p_batch_no, p_mrp,
            p_sub_unit, p_sub_unit_quantity, p_unit, p_quantity, p_rate, p_discount_per, p_amount,
            p_igst_percent, p_igst_amount, p_cgst_percent, p_cgst_amount, p_sgst_percent, p_sgst_amount,
            p_total_amount, p_crt, p_packing, p_hsn_code, p_fiscal_year_id, p_branch_id, p_created_by, CURRENT_TIMESTAMP,
            p_delete_flag, p_rate_diff, p_cogs
        )
        RETURNING *;

    ELSIF p_action = 'UPDATE' THEN
        UPDATE tab_sal_pur_tran SET
            product_id = p_product_id, product_details = p_product_details, batch_date = p_batch_date,
            batch_no = p_batch_no, mrp = p_mrp, sub_unit = p_sub_unit, sub_unit_quantity = p_sub_unit_quantity,
            unit = p_unit, quantity = p_quantity, rate = p_rate, discount_per = p_discount_per,
            amount = p_amount, total_amount = p_total_amount, rate_diff = p_rate_diff, cogs = p_cogs
        WHERE sp_tran_unique_code = p_sp_tran_unique_code;
        
        RETURN QUERY SELECT * FROM tab_sal_pur_tran WHERE sp_tran_unique_code = p_sp_tran_unique_code;

    ELSIF p_action = 'GETBYPARENT' THEN
        RETURN QUERY SELECT * FROM tab_sal_pur_tran WHERE sp_unique_code = p_sp_unique_code AND delete_flag = FALSE;
    END IF;
END;
$$ LANGUAGE plpgsql;
