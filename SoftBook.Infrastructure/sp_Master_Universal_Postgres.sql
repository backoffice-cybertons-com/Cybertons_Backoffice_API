-- PostgreSQL Universal Master Procedure (PL/pgSQL Function) - Snake Case Version
-- This function handles CRUD for all Master Entities with mandatory company_id isolation.

CREATE OR REPLACE FUNCTION public.sp_master_universal(
    p_master_name VARCHAR(100),
    p_action VARCHAR(20),
    p_company_id INT,
    p_id INT DEFAULT NULL,
    p_json_data JSONB DEFAULT NULL
) 
RETURNS SETOF JSON AS $body$
DECLARE
    v_table_name TEXT;
    v_pk_name TEXT;
    v_column_list TEXT;
    v_query TEXT;
    v_count INT;
BEGIN
    -- Mapping logical name to actual snake_case table name and primary key
    CASE LOWER(p_master_name)
        WHEN 'account', 'tabaccountmaster', 'v_account', 'accounts' THEN v_table_name := 'tab_account_master'; v_pk_name := 'account_id';
        WHEN 'accountspayable', 'v_accountspayable' THEN v_table_name := 'v_accounts_payable'; v_pk_name := 'accounts_payable_id';
        WHEN 'accountsreceivableref', 'v_accountsreceivableref' THEN v_table_name := 'v_accounts_receivable_ref'; v_pk_name := 'accounts_receivable';
        WHEN 'bank', 'tabbankmaster', 'bank_master', 'banks' THEN v_table_name := 'tab_bank_master'; v_pk_name := 'bank_master_id';
        WHEN 'branch', 'tabbranchmaster', 'branch_master', 'branches' THEN v_table_name := 'tab_branch_master'; v_pk_name := 'branch_id';
        WHEN 'customer', 'v_customer', 'customers' THEN v_table_name := 'v_customer'; v_pk_name := 'customer_id';
        WHEN 'expenseaccount', 'v_expenseaccount' THEN v_table_name := 'v_expense_account'; v_pk_name := 'expense_account_id';
        WHEN 'hsncoderef', 'v_hsncoderef' THEN v_table_name := 'v_hsn_code_ref'; v_pk_name := 'hsn_code';
        WHEN 'igstref', 'v_igstref' THEN v_table_name := 'v_igst_ref'; v_pk_name := 'igst';
        WHEN 'paidthroughaccount', 'v_paidthroughaccount' THEN v_table_name := 'v_paid_through_account'; v_pk_name := 'paid_through_account_id';
        WHEN 'sacref', 'v_sacref' THEN v_table_name := 'v_sac_ref'; v_pk_name := 'sac';
        WHEN 'vendor', 'v_vendor', 'vendors' THEN v_table_name := 'v_vendor'; v_pk_name := 'vendor_id';
        WHEN 'unit', 'tabunitmaster', 'unit_master', 'units' THEN v_table_name := 'tab_unit_master'; v_pk_name := 'unit_id';
        WHEN 'tax', 'tabtaxmaster', 'tax_master', 'taxes' THEN v_table_name := 'tab_tax_master'; v_pk_name := 'tax_id';
        WHEN 'hsn', 'tabhsnmaster', 'hsn_master' THEN v_table_name := 'tab_hsn_master'; v_pk_name := 'hsn_id';
        WHEN 'sac', 'tabsacmaster', 'sac_master', 'sacs' THEN v_table_name := 'tab_sac_master'; v_pk_name := 'sac_id';
        WHEN 'currency', 'tabcurrencymaster', 'currency_master', 'currencies' THEN v_table_name := 'tab_currency_master'; v_pk_name := 'currency_id';
        WHEN 'godown', 'tabgodownmaster', 'godown_master', 'godowns' THEN v_table_name := 'tab_godown_master'; v_pk_name := 'godown_id';
        WHEN 'warehouse', 'tabwarehousemaster', 'warehouse_master', 'wharehouse', 'warehouses' THEN v_table_name := 'tab_warehouse_master'; v_pk_name := 'warehouse_id';
        WHEN 'costcenter', 'tabcostcentermaster', 'cost_center', 'cost_center_master', 'costcenters' THEN v_table_name := 'tab_cost_center_master'; v_pk_name := 'cost_center_id';
        WHEN 'fiscalyear', 'tabfiscalyearmaster', 'fiscal_year', 'fiscal_year_master', 'fiscalyears' THEN v_table_name := 'tab_fiscal_year_master'; v_pk_name := 'fiscal_year_id';
        WHEN 'paymentmode', 'tabpaymentmodemaster', 'payment_mode', 'payment_mode_master', 'paymentmodes' THEN v_table_name := 'tab_payment_mode_master'; v_pk_name := 'payment_mode_id_internal';
        WHEN 'paymentterm', 'tabpaymenttermmaster', 'payment_term', 'payment_term_master', 'paymentterms' THEN v_table_name := 'tab_payment_term_master'; v_pk_name := 'term_id';
        WHEN 'productgroup', 'tabproductgroupmaster', 'product_group', 'productgroups' THEN v_table_name := 'tab_product_group_master'; v_pk_name := 'product_group_id';
        WHEN 'project', 'tabprojectmaster', 'project_master', 'projects' THEN v_table_name := 'tab_project_master'; v_pk_name := 'project_id';
        WHEN 'role', 'tabrolemaster', 'role_master', 'roles' THEN v_table_name := 'tab_role_master'; v_pk_name := 'role_id';
        WHEN 'salesperson', 'tabsalespersonmaster', 'salesperson_master', 'salespersons' THEN v_table_name := 'tab_salesperson_master'; v_pk_name := 'salesperson_id';
        WHEN 'transporter', 'tabtransportermaster', 'transporter_master', 'transporters' THEN v_table_name := 'tab_transporter_master'; v_pk_name := 'transporter_id';
        WHEN 'party', 'partymaster', 'tabpartymaster', 'party_master', 'parties' THEN v_table_name := 'tab_party_master'; v_pk_name := 'party_id';
        WHEN 'product', 'tabproductmaster', 'product_master', 'products' THEN v_table_name := 'tab_product_master'; v_pk_name := 'product_id';
        WHEN 'partytype', 'tabpartytypemaster', 'party_type', 'partytypes' THEN v_table_name := 'tab_party_type_master'; v_pk_name := 'party_type_id';
        WHEN 'taxgroup', 'tabtaxgroupmaster', 'tax_group', 'taxgroups' THEN v_table_name := 'tab_tax_group_master'; v_pk_name := 'tax_group_id';
        WHEN 'taxgroupdetail', 'tabtaxgroupdetail', 'configuretax', 'tax_group_detail', 'taxgroupdetails' THEN v_table_name := 'tab_tax_group_detail'; v_pk_name := 'tax_group_detail_id';
        WHEN 'tcs', 'tabtcsmaster', 'tcs_master' THEN v_table_name := 'tab_tcs_master'; v_pk_name := 'tcs_id';
        WHEN 'tds', 'tabtdsmaster', 'tds_master' THEN v_table_name := 'tab_tds_master'; v_pk_name := 'tds_id';
        WHEN 'user', 'tabusermaster', 'user_master', 'users' THEN v_table_name := 'tab_user_master'; v_pk_name := 'user_id';
        WHEN 'addresstype', 'tabaddresstypemaster', 'address_type', 'addresstypes' THEN v_table_name := 'tab_address_type_master'; v_pk_name := 'address_type_id';
        WHEN 'taxexemption', 'tabtaxexemptionmaster', 'tax_exemption', 'taxexemptions' THEN v_table_name := 'tab_tax_exemption_master'; v_pk_name := 'tax_exemption_id';
        WHEN 'taxpreference', 'tabtaxpreferencemaster', 'tax_preference', 'taxpreferences' THEN v_table_name := 'tab_tax_preference_master'; v_pk_name := 'tax_preference_id';
        ELSE RAISE EXCEPTION 'Master % not supported', p_master_name;
    END CASE;

    -- Dynamic CRUD Implementation
    IF p_action = 'SELECT' THEN
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.company_id = %L', v_table_name, p_company_id);
        
    ELSIF p_action = 'SELECT_BY_ID' THEN
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.%I = %L AND t.company_id = %L', v_table_name, v_pk_name, p_id, p_company_id);
    
    ELSIF p_action = 'SELECT_BY_FILTER' AND LOWER(p_master_name) IN ('taxgroupdetail', 'tabtaxgroupdetail', 'configuretax') THEN
        -- Special handling for TaxGroupDetail filtering by TaxGroupId
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.tax_group_id = %L AND t.company_id = %L', v_table_name, p_id, p_company_id);

    ELSIF p_action = 'INSERT' THEN
        -- Prevent insertion into views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot insert into view %', v_table_name; END IF;
        
        -- Get all writeable columns except PK and metadata
        SELECT string_agg(quote_ident(column_name), ', ') INTO v_column_list
        FROM information_schema.columns 
        WHERE table_name = v_table_name 
        AND table_schema = 'public'
        AND column_name NOT IN (v_pk_name, 'created_on', 'created_at');

        v_query := format('INSERT INTO %I (%s) SELECT %s FROM jsonb_populate_record(NULL::%I, %L || jsonb_build_object(''company_id'', %L)) RETURNING row_to_json(%I)', 
                         v_table_name, v_column_list, v_column_list, v_table_name, p_json_data, p_company_id, v_table_name);
        RETURN QUERY EXECUTE v_query;
        
    ELSIF p_action = 'UPDATE' THEN
        -- Prevent update on views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot update view %', v_table_name; END IF;
        
        -- Get all updatable columns except PK and isolation/metadata columns
        SELECT string_agg(quote_ident(column_name), ', ') INTO v_column_list
        FROM information_schema.columns 
        WHERE table_name = v_table_name 
        AND table_schema = 'public'
        AND column_name NOT IN (v_pk_name, 'company_id', 'created_on', 'created_at');

        v_query := format('UPDATE %I t SET (%s) = (SELECT %s FROM jsonb_populate_record(t, %L)) 
                          WHERE t.%I = %L AND t.company_id = %L RETURNING row_to_json(t)', 
                         v_table_name, v_column_list, v_column_list, p_json_data, v_pk_name, p_id, p_company_id);
        
        RETURN QUERY EXECUTE v_query;
                       
    ELSIF p_action = 'DELETE' THEN
        -- Prevent delete on views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot delete from view %', v_table_name; END IF;
        
        EXECUTE format('DELETE FROM %I WHERE %I = %L AND company_id = %L', v_table_name, v_pk_name, p_id, p_company_id);
        
        GET DIAGNOSTICS v_count = ROW_COUNT;
        IF v_count = 0 THEN
            RAISE EXCEPTION 'Record not found or access denied for ID % in Company %', p_id, p_company_id;
        END IF;

        RETURN NEXT json_build_object('status', 'deleted', 'id', p_id);
    END IF;

END;
$body$ LANGUAGE plpgsql;
