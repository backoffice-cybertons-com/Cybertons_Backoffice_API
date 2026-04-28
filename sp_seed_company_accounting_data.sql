-- PROCEDURE: sp_seed_company_accounting_data
-- Purpose: To seed a new or existing company with standard accounting masters (Groups, Voucher Types, and Accounts)
-- Usage: CALL sp_seed_company_accounting_data(1);

CREATE OR REPLACE PROCEDURE sp_seed_company_accounting_data(p_company_id INT)
LANGUAGE plpgsql
AS $$
DECLARE
    v_asset_grp INT; v_liab_grp INT; v_inc_grp INT; v_exp_grp INT;
    v_curr_asset_grp INT; v_curr_liab_grp INT; v_sales_grp INT; v_pur_grp INT;
    v_acc_sales INT; v_acc_pur INT; v_acc_cgst_in INT; v_acc_sgst_in INT; v_acc_igst_in INT;
    v_acc_cgst_out INT; v_acc_sgst_out INT; v_acc_igst_out INT; v_acc_round INT;
BEGIN
    -- 1. SEED ACCOUNT GROUPS
    -- Check if groups already exist to avoid duplicates
    IF NOT EXISTS (SELECT 1 FROM tab_account_group_master WHERE company_id = p_company_id) THEN
        -- Primary Groups
        INSERT INTO tab_account_group_master (company_id, group_name, group_type) VALUES (p_company_id, 'Assets', 'Asset') RETURNING group_id INTO v_asset_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type) VALUES (p_company_id, 'Liabilities', 'Liability') RETURNING group_id INTO v_liab_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type) VALUES (p_company_id, 'Income', 'Income') RETURNING group_id INTO v_inc_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type) VALUES (p_company_id, 'Expense', 'Expense') RETURNING group_id INTO v_exp_grp;

        -- Sub-Groups
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Current Assets', 'Asset', v_asset_grp) RETURNING group_id INTO v_curr_asset_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Fixed Assets', 'Asset', v_asset_grp);
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Current Liabilities', 'Liability', v_liab_grp) RETURNING group_id INTO v_curr_liab_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Sales Accounts', 'Income', v_inc_grp) RETURNING group_id INTO v_sales_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Purchase Accounts', 'Expense', v_exp_grp) RETURNING group_id INTO v_pur_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Direct Expenses', 'Expense', v_exp_grp);
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Indirect Expenses', 'Expense', v_exp_grp);
    END IF;

    -- 2. SEED VOUCHER TYPES
    IF NOT EXISTS (SELECT 1 FROM tab_voucher_type_master WHERE company_id = p_company_id) THEN
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Sales', 'SLS/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Purchase', 'PUR/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Payment', 'PAY/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Receipt', 'RCT/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Journal', 'JV/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Contra', 'CON/', 1);
    END IF;

    -- 3. SEED DEFAULT ACCOUNTS
    -- mapping: company_name = account_name, party_type = account_type, display_name = account_code
    IF NOT EXISTS (SELECT 1 FROM tab_party_master WHERE company_id = p_company_id AND company_name = 'Sales Account' AND party_type = 'Sales') THEN
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'Sales Account', 'Sales', 'ACC001') RETURNING party_id INTO v_acc_sales;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'Purchase Account', 'Purchase', 'ACC002') RETURNING party_id INTO v_acc_pur;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'CGST Input', 'Tax', 'TAX001') RETURNING party_id INTO v_acc_cgst_in;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'SGST Input', 'Tax', 'TAX002') RETURNING party_id INTO v_acc_sgst_in;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'IGST Input', 'Tax', 'TAX003') RETURNING party_id INTO v_acc_igst_in;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'CGST Output', 'Tax', 'TAX004') RETURNING party_id INTO v_acc_cgst_out;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'SGST Output', 'Tax', 'TAX005') RETURNING party_id INTO v_acc_sgst_out;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'IGST Output', 'Tax', 'TAX006') RETURNING party_id INTO v_acc_igst_out;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'Round Off', 'Other', 'ACC003') RETURNING party_id INTO v_acc_round;
        INSERT INTO tab_party_master (company_id, company_name, party_type, display_name) VALUES (p_company_id, 'Cash Account', 'Cash', 'CASH01');

        -- 4. SEED/UPDATE LEDGER SETTINGS
        IF EXISTS (SELECT 1 FROM tab_ledger_settings WHERE company_id = p_company_id) THEN
            UPDATE tab_ledger_settings SET
                sales_revenue_account_id = v_acc_sales,
                purchase_expense_account_id = v_acc_pur,
                cgst_output_account_id = v_acc_cgst_out,
                sgst_output_account_id = v_acc_sgst_out,
                igst_output_account_id = v_acc_igst_out,
                cgst_input_account_id = v_acc_cgst_in,
                sgst_input_account_id = v_acc_sgst_in,
                igst_input_account_id = v_acc_igst_in,
                round_off_account_id = v_acc_round
            WHERE company_id = p_company_id;
        ELSE
            INSERT INTO tab_ledger_settings (
                company_id, sales_revenue_account_id, purchase_expense_account_id,
                cgst_output_account_id, sgst_output_account_id, igst_output_account_id,
                cgst_input_account_id, sgst_input_account_id, igst_input_account_id,
                round_off_account_id
            ) VALUES (
                p_company_id, v_acc_sales, v_acc_pur,
                v_acc_cgst_out, v_acc_sgst_out, v_acc_igst_out,
                v_acc_cgst_in, v_acc_sgst_in, v_acc_igst_in,
                v_acc_round
            );
        END IF;
    END IF;

END $$;
