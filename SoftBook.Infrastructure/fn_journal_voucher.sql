CREATE OR REPLACE FUNCTION fn_journal_voucher(
    p_action         VARCHAR,
    p_jv_date        DATE         DEFAULT NULL,
    p_due_date       DATE         DEFAULT NULL,
    p_dr_account     BIGINT       DEFAULT NULL,
    p_cr_account     BIGINT       DEFAULT NULL,
    p_amount         NUMERIC      DEFAULT NULL,
    p_v_type         VARCHAR      DEFAULT NULL,
    p_v_code         BIGINT       DEFAULT NULL,
    p_description    TEXT         DEFAULT NULL,
    p_adjust_v_type  VARCHAR      DEFAULT NULL,
    p_jv_comp_code   INT          DEFAULT NULL,
    p_g_code         VARCHAR      DEFAULT NULL,
    p_cheque_no      VARCHAR      DEFAULT NULL,
    p_user_code      INT          DEFAULT NULL,
    p_group_jv_sr_no INT          DEFAULT NULL,
    p_is_outstanding BOOLEAN      DEFAULT FALSE,
    p_year_code      INT          DEFAULT NULL,
    p_branch_code    INT          DEFAULT NULL,
    p_created_by     VARCHAR      DEFAULT NULL,
    p_delete_flag    BOOLEAN      DEFAULT FALSE,
    p_user_id        INT          DEFAULT NULL
)
RETURNS VOID AS $$
DECLARE
    v_tab_id INT;
BEGIN
    -- 1. Identify Tab ID
    -- Journal: 40, Expense/Income: 16
    v_tab_id := CASE 
        WHEN p_v_type IN ('Expense', 'Income') THEN 16 
        ELSE 40 
    END;

    -- 2. Validate Permission (only if p_user_id is provided)
    IF p_user_id IS NOT NULL AND p_action IN ('INSERT', 'DELETE') THEN
        PERFORM public.fn_validate_permission(p_user_id, p_jv_comp_code, v_tab_id, p_action);
    END IF;

    -- 3. Perform Action
    IF p_action = 'INSERT' THEN
        INSERT INTO tab_jv (
            jv_date, due_date, dr_account_code, cr_account_code, amount, v_type, v_code, 
            description, adjust_v_type, jv_comp_code, g_code, cheque_no, user_code, 
            group_jv_sr_no, is_outstanding, year_code, branch_code, created_by, delete_flag
        )
        VALUES (
            p_jv_date, p_due_date, p_dr_account, p_cr_account, p_amount, p_v_type, p_v_code,
            p_description, p_adjust_v_type, p_jv_comp_code, p_g_code, p_cheque_no, p_user_code,
            p_group_jv_sr_no, p_is_outstanding, p_year_code, p_branch_code, p_created_by, p_delete_flag
        );

    ELSIF p_action = 'DELETE' THEN
        -- Standard delete by GCode
        UPDATE tab_jv 
        SET delete_flag = TRUE 
        WHERE g_code = p_g_code AND jv_comp_code = p_jv_comp_code;
    END IF;
END;
$$ LANGUAGE plpgsql;
