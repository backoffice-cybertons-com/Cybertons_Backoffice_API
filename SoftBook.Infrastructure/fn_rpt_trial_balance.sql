CREATE OR REPLACE FUNCTION fn_rpt_trial_balance(
    p_company_id INT,
    p_to_date DATE
)
RETURNS TABLE (
    out_account_code BIGINT,
    out_account_name VARCHAR,
    out_account_group VARCHAR,
    out_total_debit NUMERIC,
    out_total_credit NUMERIC,
    out_closing_balance NUMERIC
) AS $$
BEGIN
    RETURN QUERY
    WITH all_accounts AS (
        -- Combine Party Master and Account Master to get a single list of all ledgers
        SELECT 
            party_id::BIGINT AS id, 
            display_name AS name, 
            party_type AS grp, 
            COALESCE(opening_balance, 0) AS ob, 
            COALESCE(opening_balance_type, 'Dr') AS ob_type
        FROM tab_party_master 
        WHERE company_id = p_company_id
        
        UNION ALL
        
        SELECT 
            account_id::BIGINT AS id, 
            account_name AS name, 
            account_type AS grp, 
            0 AS ob, 
            'Dr' AS ob_type
        FROM tab_account_master 
        WHERE company_id = p_company_id
    ),
    jv_aggregated AS (
        -- Aggregate Debits and Credits from Journal Vouchers
        SELECT 
            acc, 
            SUM(dr) AS total_dr, 
            SUM(cr) AS total_cr
        FROM (
            SELECT dr_account_code AS acc, amount AS dr, 0 AS cr
            FROM tab_jv
            WHERE jv_comp_code = p_company_id 
              AND jv_date <= p_to_date 
              AND delete_flag = FALSE
            
            UNION ALL
            
            SELECT cr_account_code AS acc, 0 AS dr, amount AS cr
            FROM tab_jv
            WHERE jv_comp_code = p_company_id 
              AND jv_date <= p_to_date 
              AND delete_flag = FALSE
        ) t
        GROUP BY acc
    )
    SELECT 
        a.id, 
        a.name, 
        a.grp,
        -- Final Debit = JV Debits + Opening if Dr
        CAST(COALESCE(j.total_dr, 0) + CASE WHEN a.ob_type = 'Dr' THEN a.ob ELSE 0 END AS NUMERIC) AS total_debit,
        -- Final Credit = JV Credits + Opening if Cr
        CAST(COALESCE(j.total_cr, 0) + CASE WHEN a.ob_type = 'Cr' THEN a.ob ELSE 0 END AS NUMERIC) AS total_credit,
        -- Net Balance
        CAST((COALESCE(j.total_dr, 0) + CASE WHEN a.ob_type = 'Dr' THEN a.ob ELSE 0 END) - 
             (COALESCE(j.total_cr, 0) + CASE WHEN a.ob_type = 'Cr' THEN a.ob ELSE 0 END) AS NUMERIC) AS closing_balance
    FROM all_accounts a
    LEFT JOIN jv_aggregated j ON a.id = j.acc
    WHERE 
        COALESCE(j.total_dr, 0) <> 0 OR 
        COALESCE(j.total_cr, 0) <> 0 OR 
        a.ob <> 0
    ORDER BY a.grp, a.name;
END;
$$ LANGUAGE plpgsql;
