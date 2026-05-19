-- Migration to store quantity fields with four decimal places.
-- tab_jv is not included because journal vouchers do not store quantity columns.
DO $$
DECLARE
    qty_column record;
BEGIN
    FOR qty_column IN
        SELECT *
        FROM (VALUES
            ('tab_product_master', 'opening_quantity', 'NUMERIC(18,4)'),
            ('tab_product_master', 'min_quantity', 'NUMERIC(18,4)'),
            ('tab_product_master', 'max_quantity', 'NUMERIC(18,4)'),
            ('tab_product_master', 're_order_quantity', 'NUMERIC(18,4)'),
            ('tab_product_master', 'reorder_quantity', 'NUMERIC(18,4)'),
            ('tab_sale_pur', 'total_quantity', 'NUMERIC(18,4)'),
            ('tab_sal_pur_tran', 'sub_unit_quantity', 'NUMERIC(18,4)'),
            ('tab_sal_pur_tran', 'quantity', 'NUMERIC(18,4)'),
            ('tab_bill_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_credit_notes_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_delivery_challan_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_eway_bills_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_invoice_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_purchase_order_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_quote_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_recurring_bill_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_recurring_invoice_items', 'quantity', 'NUMERIC(18,4)'),
            ('tab_sales_order_items', 'quantity', 'NUMERIC(18,4)')
        ) AS v(table_name, column_name, data_type)
    LOOP
        IF to_regclass(qty_column.table_name) IS NOT NULL
           AND EXISTS (
               SELECT 1
               FROM information_schema.columns
               WHERE table_schema = current_schema()
                 AND table_name = qty_column.table_name
                 AND column_name = qty_column.column_name
           ) THEN
            EXECUTE format(
                'ALTER TABLE %I ALTER COLUMN %I TYPE %s USING ROUND(%I::numeric, 4)',
                qty_column.table_name,
                qty_column.column_name,
                qty_column.data_type,
                qty_column.column_name
            );

            EXECUTE format(
                'ALTER TABLE %I ALTER COLUMN %I SET DEFAULT 0.0000',
                qty_column.table_name,
                qty_column.column_name
            );
        END IF;
    END LOOP;
END $$;
