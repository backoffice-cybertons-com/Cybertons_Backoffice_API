-- Drop both potentially existing versions to avoid ambiguity errors
DROP FUNCTION IF EXISTS public.sp_master_universal(character varying, character varying, integer, integer, jsonb);
DROP FUNCTION IF EXISTS public.sp_master_universal(character varying, character varying, integer, integer, jsonb, integer);

CREATE OR REPLACE FUNCTION public.sp_master_universal(
    p_master_name VARCHAR(100),
    p_action VARCHAR(20),
    p_company_id INT,
    p_id INT DEFAULT NULL,
    p_json_data JSONB DEFAULT NULL,
    p_user_id INT DEFAULT NULL
) 
RETURNS SETOF JSON AS $body$
DECLARE
    v_table_name TEXT;
    v_pk_name TEXT;
    v_column_list TEXT;
    v_query TEXT;
    v_count INT;
    v_tab_id INT;
    v_can_add BOOLEAN;
    v_can_edit BOOLEAN;
BEGIN
    -- Authorization Check for Modifying Actions
    IF p_action IN ('INSERT', 'UPDATE', 'DELETE') THEN
        -- 1. Identify valid Tab ID for the requested Master
        SELECT tab_id INTO v_tab_id FROM tabs 
        WHERE LOWER(tab_name) = LOWER(p_master_name)
           OR LOWER(tab_url) LIKE '%' || LOWER(p_master_name) || '%'
        LIMIT 1;

        -- 1a. Fallback: If no specific tab found, use the general "Masters" tab (ID 1011)
        IF v_tab_id IS NULL THEN
            v_tab_id := 1011; -- Default to general Masters permission
        END IF;

        -- 2. Validate Permission (only if p_user_id is provided)
        IF p_user_id IS NOT NULL THEN
            PERFORM public.fn_validate_permission(
                p_user_id, 
                p_company_id, 
                v_tab_id, 
                CASE 
                    WHEN p_action = 'INSERT' THEN 'ADD'
                    WHEN p_action = 'UPDATE' THEN 'EDIT'
                    WHEN p_action = 'DELETE' THEN 'DELETE'
                END
            );
        END IF;
    END IF;
    -- Mapping logical name to actual snake_case table name and primary key
    CASE LOWER(p_master_name)
        WHEN 'account', 'tabaccountmaster', 'v_account', 'accounts' THEN v_table_name := 'tab_party_master'; v_pk_name := 'party_id';
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
        WHEN 'ledgersetting', 'ledger_settings', 'tab_ledger_settings' THEN v_table_name := 'tab_ledger_settings'; v_pk_name := 'setting_id';
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
        WHEN 'vehicle', 'tabvehiclemaster', 'vehicle_master', 'vehicles' THEN v_table_name := 'tab_vehicle_master'; v_pk_name := 'vehicle_id';
        WHEN 'accountgroup', 'tabaccountgroupmaster', 'account_group', 'accountgroups' THEN v_table_name := 'tab_account_group_master'; v_pk_name := 'group_id';
        WHEN 'vouchertype', 'tabvouchertypemaster', 'voucher_type', 'vouchertypes' THEN v_table_name := 'tab_voucher_type_master'; v_pk_name := 'v_type_id';
        WHEN 'ledgersetting', 'tabledgersettings', 'ledger_setting', 'ledgersettings' THEN v_table_name := 'tab_ledger_settings'; v_pk_name := 'setting_id';
        ELSE RAISE EXCEPTION 'Master % not supported', p_master_name;
    END CASE;

    -- Dynamic CRUD Implementation
    IF p_action = 'SELECT' THEN
        IF LOWER(p_master_name) IN ('account', 'tabaccountmaster', 'v_account', 'accounts') THEN
            -- Only show accounts (exclude real parties as requested)
            RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.company_id = %L AND t.party_type NOT IN (''Customer'', ''Vendor'', ''Trader'', ''Transporter'')', v_table_name, p_company_id);
        ELSIF LOWER(p_master_name) IN ('user', 'tabusermaster', 'user_master', 'users') THEN
            -- Join with role tables to show role info in the list
            RETURN QUERY EXECUTE format('
                SELECT row_to_json(j) FROM (
                    SELECT u.*, r.role_id, rm.role_name 
                    FROM tab_user_master u
                    LEFT JOIN tab_user_role r ON u.user_id = r.user_id AND u.company_id = r.company_id
                    LEFT JOIN tab_role_master rm ON r.role_id = rm.role_id AND r.company_id = rm.company_id
                    WHERE u.company_id = %L
                ) j', p_company_id);
       
        ELSIF LOWER(p_master_name) IN ('product', 'tabproductmaster', 'product_master', 'products') THEN
            -- Join with master tables to show names for Group, HSN, and Tax Group
            RETURN QUERY EXECUTE format('
                SELECT row_to_json(j) FROM (
                    SELECT t.*, 
                           pg.group_name AS "GroupName", 
                           tg.group_name AS "TaxGroupName", 
                           h.hsn_code AS "HsnCode"
                    FROM tab_product_master t
                    LEFT JOIN tab_product_group_master pg ON t.product_group_id = pg.product_group_id AND t.company_id = pg.company_id
                    LEFT JOIN tab_tax_group_master tg ON t.tax_group_id = tg.tax_group_id AND t.company_id = tg.company_id
                    LEFT JOIN tab_hsn_master h ON t.hsn_id = h.hsn_id AND t.company_id = h.company_id
                    WHERE t.company_id = %L
                ) j', p_company_id);
       
       ELSE
            RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.company_id = %L', v_table_name, p_company_id);
        END IF;
        
    ELSIF p_action = 'SELECT_BY_ID' THEN
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.%I = %L AND t.company_id = %L', v_table_name, v_pk_name, p_id, p_company_id);
    
    ELSIF p_action = 'SELECT_BY_FILTER' AND LOWER(p_master_name) IN ('taxgroupdetail', 'tabtaxgroupdetail', 'configuretax') THEN
        -- Special handling for TaxGroupDetail filtering by TaxGroupId
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.tax_group_id = %L AND t.company_id = %L', v_table_name, p_id, p_company_id);

    ELSIF p_action = 'SELECT_BY_FILTER' AND LOWER(p_master_name) IN ('vehicle', 'vehicles', 'tabvehiclemaster') THEN
        -- Special handling for Vehicle filtering by TransporterId
        RETURN QUERY EXECUTE format('SELECT row_to_json(t) FROM %I t WHERE t.transporter_id = %L AND t.company_id = %L', v_table_name, p_id, p_company_id);

    ELSIF p_action = 'INSERT' THEN
        -- Special handling: fiscal year uses validation + overlap checks
        IF LOWER(p_master_name) IN ('fiscalyear', 'tabfiscalyearmaster', 'fiscal_year', 'fiscal_year_master', 'fiscalyears') THEN
            RETURN QUERY
            SELECT row_to_json(t)
            FROM public.fn_fiscal_year(
                'INSERT',
                NULL,
                p_company_id,
                (p_json_data->>'start_date')::date,
                (p_json_data->>'end_date')::date,
                COALESCE((p_json_data->>'is_current')::boolean, FALSE)
            ) t;
            RETURN;
        END IF;

        -- Prevent insertion into views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot insert into view %', v_table_name; END IF;
        
        -- Get all writeable columns except PK and metadata
        SELECT string_agg(quote_ident(column_name), ', ') INTO v_column_list
        FROM information_schema.columns 
        WHERE table_name = v_table_name 
        AND table_schema = 'public'
        AND column_name NOT IN (v_pk_name, 'created_on', 'created_at');

        -- Special handling for Users: insert into tab_user_master AND tab_user_role
        IF v_table_name = 'tab_user_master' THEN
            DECLARE
                v_new_row_json JSONB;
                v_new_user_id INT;
                v_role_id INT := (p_json_data->>'role_id')::INT;
            BEGIN
                v_query := format('INSERT INTO %I (%s) SELECT %s FROM jsonb_populate_record(NULL::%I, %L || jsonb_build_object(''company_id'', %L)) RETURNING row_to_json(%I)', 
                                 v_table_name, v_column_list, v_column_list, v_table_name, p_json_data, p_company_id, v_table_name);
                EXECUTE v_query INTO v_new_row_json;
                v_new_user_id := (v_new_row_json->>'user_id')::INT;
                
                IF v_role_id IS NOT NULL THEN
                    INSERT INTO tab_user_role (user_id, role_id, company_id) VALUES (v_new_user_id, v_role_id, p_company_id);
                END IF;
                
                RETURN NEXT v_new_row_json;
                RETURN;
            END;
        END IF;

        v_query := format('INSERT INTO %I (%s) SELECT %s FROM jsonb_populate_record(NULL::%I, %L || jsonb_build_object(''company_id'', %L)) RETURNING row_to_json(%I)', 
                         v_table_name, v_column_list, v_column_list, v_table_name, p_json_data, p_company_id, v_table_name);
        RETURN QUERY EXECUTE v_query;
        
    ELSIF p_action = 'UPDATE' THEN
        -- Special handling: fiscal year uses validation + overlap checks
        IF LOWER(p_master_name) IN ('fiscalyear', 'tabfiscalyearmaster', 'fiscal_year', 'fiscal_year_master', 'fiscalyears') THEN
            RETURN QUERY
            SELECT row_to_json(t)
            FROM public.fn_fiscal_year(
                'UPDATE',
                p_id,
                p_company_id,
                (p_json_data->>'start_date')::date,
                (p_json_data->>'end_date')::date,
                COALESCE((p_json_data->>'is_current')::boolean, FALSE)
            ) t;
            RETURN;
        END IF;

        -- Prevent update on views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot update view %', v_table_name; END IF;
        
        -- Get all updatable columns except PK and isolation/metadata columns
        SELECT string_agg(quote_ident(column_name), ', ') INTO v_column_list
        FROM information_schema.columns 
        WHERE table_name = v_table_name 
        AND table_schema = 'public'
        AND column_name NOT IN (v_pk_name, 'company_id', 'created_on', 'created_at');

        -- Special handling for Users: update tab_user_master AND tab_user_role
        IF v_table_name = 'tab_user_master' THEN
            DECLARE
                v_updated_row_json JSONB;
                v_role_id INT := (p_json_data->>'role_id')::INT;
            BEGIN
                v_query := format('UPDATE %I SET (%s, company_id) = (SELECT %s, %L::integer FROM jsonb_populate_record(NULL::%I, %L)) 
                                  WHERE %I = %L AND (company_id = %L OR company_id = 0) RETURNING row_to_json(%I)', 
                                 v_table_name, v_column_list, v_column_list, p_company_id, v_table_name, p_json_data, v_pk_name, p_id, p_company_id, v_table_name);
                EXECUTE v_query INTO v_updated_row_json;
                
                IF v_role_id IS NOT NULL THEN
                    DELETE FROM tab_user_role WHERE user_id = p_id AND company_id = p_company_id;
                    INSERT INTO tab_user_role (user_id, role_id, company_id) VALUES (p_id, v_role_id, p_company_id);
                END IF;

                RETURN NEXT v_updated_row_json;
                RETURN;
            END;
        END IF;

        v_query := format('UPDATE %I SET (%s) = (SELECT %s FROM jsonb_populate_record(NULL::%I, %L)) 
                          WHERE %I = %L AND company_id = %L RETURNING row_to_json(%I)', 
                         v_table_name, v_column_list, v_column_list, v_table_name, p_json_data, v_pk_name, p_id, p_company_id, v_table_name);
        
        RETURN QUERY EXECUTE v_query;
                       
    ELSIF p_action = 'DELETE' THEN
        -- Prevent delete on views
        IF v_table_name LIKE 'v_%' THEN RAISE EXCEPTION 'Cannot delete from view %', v_table_name; END IF;
        
        -- Special handling for Users: delete roles first then user
        IF v_table_name = 'tab_user_master' THEN
            DELETE FROM tab_user_role WHERE user_id = p_id AND company_id = p_company_id;
        END IF;

        EXECUTE format('DELETE FROM %I WHERE %I = %L AND company_id = %L', v_table_name, v_pk_name, p_id, p_company_id);
        
        GET DIAGNOSTICS v_count = ROW_COUNT;
        IF v_count = 0 THEN
            RAISE EXCEPTION 'Record not found or access denied for ID % in Company %', p_id, p_company_id;
        END IF;

        RETURN NEXT json_build_object('status', 'deleted', 'id', p_id);
    END IF;

END;
$body$ LANGUAGE plpgsql;
