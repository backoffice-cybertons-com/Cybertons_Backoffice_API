-- Dashboard goods quantity summary (total + item-wise purchase/sales qty)
-- Uses base tables directly so results match sale/purchase transactions.
CREATE OR REPLACE FUNCTION public.fn_dashboard_goods_summary(
    p_company_id INT,
    p_from_date DATE,
    p_to_date DATE,
    p_fiscal_year_id INT DEFAULT NULL
)
RETURNS JSON
LANGUAGE plpgsql
AS $$
DECLARE
    v_result JSON;
BEGIN
    SELECT json_build_object(
        'goods_summary', (
            SELECT json_build_object(
                'total_purchase_qty', COALESCE(SUM(
                    CASE WHEN LOWER(TRIM(h.inv_type)) = 'purchase' THEN COALESCE(t.quantity, 0) ELSE 0 END
                ), 0),
                'total_sales_qty', COALESCE(SUM(
                    CASE WHEN LOWER(TRIM(h.inv_type)) = 'sales' THEN COALESCE(t.quantity, 0) ELSE 0 END
                ), 0)
            )
            FROM public.tab_sal_pur_tran t
            INNER JOIN public.tab_sale_pur h
                ON t.sp_unique_code = h.sp_unique_code
            WHERE h.company_id = p_company_id
              AND COALESCE(h.delete_flag, FALSE) = FALSE
              AND COALESCE(t.delete_flag, FALSE) = FALSE
              AND h.inv_date::date BETWEEN p_from_date AND p_to_date
              AND (p_fiscal_year_id IS NULL OR h.fiscal_year_id = p_fiscal_year_id)
        ),
        'item_wise_goods', COALESCE((
            SELECT json_agg(row_to_json(sub) ORDER BY sub.purchase_qty + sub.sales_qty DESC, sub.product_name)
            FROM (
                SELECT
                    COALESCE(t.product_id, 0) AS product_id,
                    COALESCE(
                        NULLIF(TRIM(p.product_name), ''),
                        NULLIF(TRIM(t.product_details), ''),
                        'Unknown Item'
                    ) AS product_name,
                    COALESCE(SUM(
                        CASE WHEN LOWER(TRIM(h.inv_type)) = 'purchase' THEN COALESCE(t.quantity, 0) ELSE 0 END
                    ), 0) AS purchase_qty,
                    COALESCE(SUM(
                        CASE WHEN LOWER(TRIM(h.inv_type)) = 'sales' THEN COALESCE(t.quantity, 0) ELSE 0 END
                    ), 0) AS sales_qty
                FROM public.tab_sal_pur_tran t
                INNER JOIN public.tab_sale_pur h
                    ON t.sp_unique_code = h.sp_unique_code
                LEFT JOIN public.tab_product_master p
                    ON t.product_id = p.product_id
                   AND p.company_id = h.company_id
                WHERE h.company_id = p_company_id
                  AND COALESCE(h.delete_flag, FALSE) = FALSE
                  AND COALESCE(t.delete_flag, FALSE) = FALSE
                  AND h.inv_date::date BETWEEN p_from_date AND p_to_date
                  AND (p_fiscal_year_id IS NULL OR h.fiscal_year_id = p_fiscal_year_id)
                GROUP BY t.product_id, p.product_name, t.product_details
                HAVING COALESCE(SUM(COALESCE(t.quantity, 0)), 0) > 0
            ) sub
        ), '[]'::json)
    )
    INTO v_result;

    RETURN COALESCE(v_result, '{"goods_summary":{"total_purchase_qty":0,"total_sales_qty":0},"item_wise_goods":[]}'::json);
END;
$$;
