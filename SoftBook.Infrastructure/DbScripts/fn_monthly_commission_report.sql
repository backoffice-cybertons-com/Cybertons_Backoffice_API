DROP FUNCTION IF EXISTS public.fn_monthly_commission_report(INT, DATE, DATE, INT, VARCHAR);

CREATE OR REPLACE FUNCTION public.fn_monthly_commission_report(
    p_company_id INT,
    p_from_date DATE,
    p_to_date DATE,
    p_party_id INT DEFAULT NULL,
    p_group_by VARCHAR DEFAULT 'date'
)
RETURNS TABLE (
    out_party_id INT,
    out_display_name VARCHAR,
    out_d DATE,
    out_inv_no VARCHAR,
    out_product_name VARCHAR,
    out_crate_ordered NUMERIC,
    out_total_mrp NUMERIC,
    out_avg_mrp NUMERIC,
    out_seven_days_avg_order NUMERIC,
    out_avg_depo_price NUMERIC,
    out_total_depo_amt NUMERIC,
    out_tot_comm NUMERIC,
    out_avg_comm_per_crate NUMERIC,
    out_os_as_on NUMERIC,
    out_payment NUMERIC,
    out_avg_term NUMERIC,
    out_adjust_commission NUMERIC,
    out_net_commission NUMERIC,
    out_comm_amount NUMERIC
) AS $$
BEGIN
    RETURN QUERY
    WITH date_range AS (
        SELECT generate_series(
            p_from_date::date - interval '7 days',
            p_to_date::date,
            '1 day'::interval
        )::date AS range_date
    ),

    parties AS (
        SELECT 
            p.party_id,
            p.display_name,
            COALESCE(p.accounts_receivable_account, p.party_id) AS ar_account,
            COALESCE(p.accounts_payable_account, p.party_id) AS ap_account,
            CASE
                WHEN UPPER(COALESCE(p.opening_balance_type, '')) = 'DR'
                    THEN COALESCE(p.opening_balance, 0)::NUMERIC
                WHEN UPPER(COALESCE(p.opening_balance_type, '')) = 'CR'
                    THEN -COALESCE(p.opening_balance, 0)::NUMERIC
                ELSE 0::NUMERIC
            END AS opening_balance
        FROM public.tab_party_master p
        WHERE p.company_id = p_company_id
          AND LOWER(TRIM(COALESCE(p.party_type, ''))) = 'customer'
          AND (p_party_id IS NULL OR p.party_id = p_party_id)
    ),

    -- Base transactions
    -- BOTH header and transaction must be active
    base_orders AS (
        SELECT
            s.party_id,
            s.inv_date::date AS order_date,
            s.inv_no,
            pr.product_name,
            COALESCE(t.crt, t.quantity) AS qty,
            COALESCE(t.mrp, 0) AS mrp,
            COALESCE(pr.selling_price, 0) AS selling_price,
            COALESCE(t.igst_amount, 0)
                + COALESCE(t.cgst_amount, 0)
                + COALESCE(t.sgst_amount, 0) AS line_gst
        FROM public.tab_sale_pur s

        INNER JOIN public.tab_sal_pur_tran t
            ON s.sp_unique_code = t.sp_unique_code
           AND t.company_id = p_company_id
           AND COALESCE(t.delete_flag, FALSE) = FALSE

        LEFT JOIN public.tab_product_master pr
            ON t.product_id = pr.product_id
           AND pr.company_id = p_company_id

        WHERE s.company_id = p_company_id
          AND LOWER(TRIM(s.inv_type)) IN ('sales', 'sale')
          AND s.inv_date::date >= (p_from_date::date - interval '7 days')
          AND s.inv_date::date <= p_to_date::date
          AND COALESCE(s.delete_flag, FALSE) = FALSE
          AND (p_party_id IS NULL OR s.party_id = p_party_id)
    ),

    -- Grouped orders for display
    daily_orders_display AS (
        SELECT
            base_orders.party_id,
            base_orders.order_date,
            CASE 
                WHEN p_group_by IN ('invoice', 'item') 
                THEN base_orders.inv_no 
                ELSE NULL 
            END AS inv_no,
            CASE 
                WHEN p_group_by = 'item' 
                THEN base_orders.product_name 
                ELSE NULL 
            END AS product_name,
            SUM(base_orders.qty) AS crate_ordered,
            SUM(base_orders.qty * base_orders.mrp) AS total_mrp,
            SUM(
                (base_orders.qty * base_orders.selling_price)
                + base_orders.line_gst
            ) AS total_depo_amt
        FROM base_orders
        GROUP BY 
            base_orders.party_id,
            base_orders.order_date,
            CASE 
                WHEN p_group_by IN ('invoice', 'item') 
                THEN base_orders.inv_no 
                ELSE NULL 
            END,
            CASE 
                WHEN p_group_by = 'item' 
                THEN base_orders.product_name 
                ELSE NULL 
            END
    ),

    -- Aggregated daily orders
    daily_orders_total AS (
        SELECT
            base_orders.party_id,
            base_orders.order_date,
            SUM(base_orders.qty) AS crate_ordered,
            SUM(base_orders.qty * base_orders.mrp) AS total_mrp,
            SUM(
                (base_orders.qty * base_orders.selling_price)
                + base_orders.line_gst
            ) AS total_depo_amt
        FROM base_orders
        GROUP BY 
            base_orders.party_id,
            base_orders.order_date
    ),

    daily_payments AS (
        SELECT
            p.party_id,
            j.jv_date::date AS payment_date,
            SUM(j.amount) AS payment_amt
        FROM public.tab_jv j
        INNER JOIN parties p 
            ON j.cr_account_code IN (p.ar_account, p.ap_account)
        WHERE j.jv_comp_code = p_company_id
          AND j.jv_date::date >= (p_from_date::date - interval '7 days')
          AND j.jv_date::date <= p_to_date::date
        GROUP BY 
            p.party_id,
            j.jv_date::date
    ),

    historical_os AS (
        SELECT
            p.party_id,
            p.opening_balance +
            COALESCE((
                SELECT SUM(
                    COALESCE(t.crt, t.quantity)
                    * COALESCE(t.mrp, 0)
                )
                FROM public.tab_sale_pur s2

                INNER JOIN public.tab_sal_pur_tran t
                    ON s2.sp_unique_code = t.sp_unique_code
                   AND t.company_id = p_company_id
                   AND COALESCE(t.delete_flag, FALSE) = FALSE

                WHERE s2.company_id = p_company_id
                  AND LOWER(TRIM(s2.inv_type)) IN ('sales', 'sale')
                  AND s2.inv_date::date < (
                      p_from_date::date - interval '7 days'
                  )
                  AND COALESCE(s2.delete_flag, FALSE) = FALSE
                  AND s2.party_id = p.party_id
            ), 0)
            -
            COALESCE((
                SELECT SUM(j2.amount)
                FROM public.tab_jv j2
                WHERE j2.jv_comp_code = p_company_id
                  AND j2.cr_account_code IN (
                      p.ar_account,
                      p.ap_account
                  )
                  AND j2.jv_date::date < (
                      p_from_date::date - interval '7 days'
                  )
            ), 0) AS os_before_7_days

        FROM parties p
    ),

    party_dates AS (
        SELECT 
            p.party_id,
            p.display_name,
            dr.range_date
        FROM parties p
        CROSS JOIN date_range dr
    ),

    daily_stats_calendar AS (
        SELECT
            pd.party_id,
            pd.display_name,
            pd.range_date,
            COALESCE(d_ord.crate_ordered, 0) AS crate_ordered,
            COALESCE(d_ord.total_mrp, 0) AS total_mrp,
            COALESCE(d_ord.total_depo_amt, 0) AS total_depo_amt,
            COALESCE(dp.payment_amt, 0) AS payment,
            ho.os_before_7_days
        FROM party_dates pd

        LEFT JOIN daily_orders_total d_ord
            ON pd.party_id = d_ord.party_id
           AND pd.range_date = d_ord.order_date

        LEFT JOIN daily_payments dp
            ON pd.party_id = dp.party_id
           AND pd.range_date = dp.payment_date

        LEFT JOIN historical_os ho
            ON pd.party_id = ho.party_id
    ),

    rolling_stats AS (
        SELECT
            ds.party_id,
            ds.display_name,
            ds.range_date,
            ds.payment,

            COALESCE((
                SELECT AVG(sub.total_mrp)
                FROM (
                    SELECT 
                        SUM(
                            COALESCE(t2.crt, t2.quantity)
                            * COALESCE(t2.mrp, 0)
                        ) AS total_mrp

                    FROM public.tab_sale_pur s2

                    INNER JOIN public.tab_sal_pur_tran t2
                        ON s2.sp_unique_code = t2.sp_unique_code
                       AND t2.company_id = p_company_id
                       AND COALESCE(t2.delete_flag, FALSE) = FALSE

                    WHERE s2.company_id = p_company_id
                      AND s2.party_id = ds.party_id
                      AND LOWER(TRIM(s2.inv_type)) IN ('sales', 'sale')
                      AND s2.inv_date::date < ds.range_date
                      AND COALESCE(s2.delete_flag, FALSE) = FALSE

                    GROUP BY s2.inv_date::date

                    HAVING SUM(
                        COALESCE(t2.crt, t2.quantity)
                        * COALESCE(t2.mrp, 0)
                    ) > 0

                    ORDER BY s2.inv_date::date DESC
                    LIMIT 7
                ) sub
            ), 0) AS seven_days_avg_order,

            ds.os_before_7_days +
            COALESCE(
                SUM(ds.total_mrp - ds.payment)
                OVER (
                    PARTITION BY ds.party_id
                    ORDER BY ds.range_date
                    ROWS BETWEEN UNBOUNDED PRECEDING
                    AND 1 PRECEDING
                ),
                0
            ) AS os_as_on

        FROM daily_stats_calendar ds
    ),

    daily_stats_calc AS (
        SELECT
            rs.party_id,
            rs.display_name,
            rs.range_date AS calc_d,
            rs.seven_days_avg_order,
            rs.os_as_on,
            rs.payment,

            CASE
                WHEN rs.seven_days_avg_order = 0 
                    THEN 0
                ELSE rs.os_as_on / rs.seven_days_avg_order
            END AS avg_term

        FROM rolling_stats rs
        WHERE rs.range_date >= p_from_date::date
    ),

    daily_stats_with_adjust AS (
        SELECT
            c.*,

            COALESCE((
                SELECT ap.add_price
                FROM public.tab_add_price ap
                WHERE c.avg_term >= COALESCE(ap.from_due, 0)
                  AND c.avg_term <= COALESCE(ap.to_due, 999999)
                ORDER BY ap.from_due DESC
                LIMIT 1
            ), 0) AS adjust_commission

        FROM daily_stats_calc c
    ),

    final_output AS (
        SELECT
            dsc.party_id,
            dsc.display_name,
            dsc.calc_d,
            dod.inv_no,
            dod.product_name,
            COALESCE(dod.crate_ordered, 0) AS crate_ordered,
            COALESCE(dod.total_mrp, 0) AS total_mrp,
            COALESCE(dod.total_depo_amt, 0) AS total_depo_amt,
            dsc.seven_days_avg_order,
            dsc.os_as_on,
            dsc.payment,
            dsc.avg_term,
            dsc.adjust_commission,

            ROW_NUMBER() OVER (
                PARTITION BY dsc.party_id, dsc.calc_d
                ORDER BY dod.inv_no, dod.product_name
            ) AS rn

        FROM daily_stats_with_adjust dsc

        LEFT JOIN daily_orders_display dod
            ON dsc.party_id = dod.party_id
           AND dsc.calc_d = dod.order_date
    ),

    calculated_final AS (
        SELECT
            final_output.party_id,
            final_output.display_name,
            final_output.calc_d,
            final_output.inv_no,
            final_output.product_name,
            final_output.crate_ordered,
            final_output.total_mrp,

            CASE
                WHEN final_output.crate_ordered = 0 
                    THEN 0
                ELSE final_output.total_mrp
                     / final_output.crate_ordered
            END AS avg_mrp,

            CASE
                WHEN final_output.rn = 1
                    THEN final_output.seven_days_avg_order
                ELSE 0
            END AS seven_days_avg_order,

            CASE
                WHEN final_output.crate_ordered = 0 
                    THEN 0
                ELSE final_output.total_depo_amt
                     / final_output.crate_ordered
            END AS avg_depo_price,

            final_output.total_depo_amt,

            final_output.total_mrp
                - final_output.total_depo_amt AS tot_comm,

            CASE
                WHEN final_output.crate_ordered = 0 
                    THEN 0
                ELSE (
                    final_output.total_mrp
                    - final_output.total_depo_amt
                ) / final_output.crate_ordered
            END AS avg_comm_per_crate,

            CASE
                WHEN final_output.rn = 1
                    THEN final_output.os_as_on
                ELSE 0
            END AS os_as_on,

            CASE
                WHEN final_output.rn = 1
                    THEN final_output.payment
                ELSE 0
            END AS payment,

            CASE
                WHEN final_output.rn = 1
                    THEN final_output.avg_term
                ELSE 0
            END AS avg_term,

            final_output.adjust_commission,

            (
                CASE
                    WHEN final_output.crate_ordered = 0 
                        THEN 0
                    ELSE (
                        final_output.total_mrp
                        - final_output.total_depo_amt
                    ) / final_output.crate_ordered
                END
                - final_output.adjust_commission
            ) AS net_commission,

            (
                CASE
                    WHEN final_output.crate_ordered = 0 
                        THEN 0
                    ELSE (
                        final_output.total_mrp
                        - final_output.total_depo_amt
                    ) / final_output.crate_ordered
                END
                - final_output.adjust_commission
            ) * final_output.crate_ordered AS comm_amount

        FROM final_output
    )

    SELECT
        calculated_final.party_id,
        calculated_final.display_name,
        calculated_final.calc_d,
        calculated_final.inv_no,
        calculated_final.product_name,
        calculated_final.crate_ordered,
        calculated_final.total_mrp,
        calculated_final.avg_mrp,
        calculated_final.seven_days_avg_order,
        calculated_final.avg_depo_price,
        calculated_final.total_depo_amt,
        calculated_final.tot_comm,
        calculated_final.avg_comm_per_crate,
        calculated_final.os_as_on,
        calculated_final.payment,
        calculated_final.avg_term,
        calculated_final.adjust_commission,
        calculated_final.net_commission,
        calculated_final.comm_amount

    FROM calculated_final

    ORDER BY
        calculated_final.party_id,
        calculated_final.calc_d,
        calculated_final.inv_no,
        calculated_final.product_name;

END;
$$ LANGUAGE plpgsql;
