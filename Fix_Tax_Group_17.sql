-- Fix CGST/SGST for Tax Group 17
-- User specified: ID 17 = GST 5% (CGST 2.5, SGST 2.5)
-- Note: User mentioned 2.25, but 2.5+2.5=5. I will set to 2.5/2.5 to be safe for 5% GST.

UPDATE tab_tax_group_master 
SET cgst_per = 2.5, 
    sgst_per = 2.5,
    igst_per = 5.0,
    group_name = 'GST 5%'
WHERE tax_group_id = 17;

-- Also verify if any products on company 10001 are using it
SELECT * FROM tab_product_master WHERE tax_group_id = 17;
