-- =============================================================================
-- Clone company_id 10001 → 10004 on softaccdb_1_3107
-- =============================================================================
-- WARNING:
--   1) BACK UP the tenant DB first.
--   2) WIPES all existing rows for company_id = 10004 (and jv_comp_code=10004),
--      then copies from 10001 with new PKs + FK remapping.
--   3) Does NOT copy shared menu table `tabs`.
--   4) Does NOT touch companymaster.
--   5) After clone, login to 10004 uses COPIED users/passwords from 10001.
--
-- Run on: softaccdb_1_3107 (tenant)
-- If anything fails: ROLLBACK;  (script ends with COMMIT)
-- =============================================================================

BEGIN;

DO $$
BEGIN
    RAISE NOTICE '============================================================';
    RAISE NOTICE 'CLONE COMPANY 10001 → 10004  (softaccdb tenant)';
    RAISE NOTICE 'Existing destination rows will be DELETED first.';
    RAISE NOTICE '============================================================';
END $$;

-- -----------------------------------------------------------------------------
-- Helpers
-- -----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION pg_temp.company_tables()
RETURNS TABLE(table_name text, company_col text)
LANGUAGE sql
AS $$
    SELECT c.table_name::text, 'company_id'::text
    FROM information_schema.columns c
    JOIN information_schema.tables t
      ON t.table_schema = c.table_schema AND t.table_name = c.table_name
    WHERE c.table_schema = 'public'
      AND c.column_name = 'company_id'
      AND t.table_type = 'BASE TABLE'
      AND c.table_name NOT IN ('tabs', 'tab_company_master')
    UNION ALL
    SELECT 'tab_jv'::text, 'jv_comp_code'::text
    WHERE EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'tab_jv' AND column_name = 'jv_comp_code'
    )
      AND NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'tab_jv' AND column_name = 'company_id'
    );
$$;

CREATE OR REPLACE FUNCTION pg_temp.table_exists(p_table text)
RETURNS boolean LANGUAGE sql AS $$
    SELECT to_regclass('public.' || p_table) IS NOT NULL;
$$;

CREATE OR REPLACE FUNCTION pg_temp.has_col(p_table text, p_col text)
RETURNS boolean LANGUAGE sql AS $$
    SELECT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = p_table AND column_name = p_col
    );
$$;

CREATE TEMP TABLE map_generic (
    entity TEXT NOT NULL,
    old_id BIGINT NOT NULL,
    new_id BIGINT NOT NULL,
    PRIMARY KEY (entity, old_id)
) ON COMMIT DROP;

CREATE INDEX ON map_generic (entity, new_id);

-- Clone one table: omit PK, rewrite company col, remap FKs via map_generic
CREATE OR REPLACE FUNCTION pg_temp.clone_table(
    p_table text,
    p_pk text,
    p_src int,
    p_dst int,
    p_map_entity text DEFAULT NULL,
    p_company_col text DEFAULT 'company_id',
    p_fk_maps text[][] DEFAULT NULL,
    p_null_self_fk text DEFAULT NULL
)
RETURNS bigint
LANGUAGE plpgsql
AS $$
DECLARE
    v_cols text[];
    v_col text;
    v_insert_cols text := '';
    v_select_exprs text := '';
    v_expr text;
    v_found boolean;
    v_i int;
    v_sql text;
    v_count bigint := 0;
BEGIN
    IF NOT pg_temp.table_exists(p_table) THEN
        RAISE NOTICE 'Skip missing table %', p_table;
        RETURN 0;
    END IF;
    IF NOT pg_temp.has_col(p_table, p_company_col) THEN
        RAISE NOTICE 'Skip % (no %)', p_table, p_company_col;
        RETURN 0;
    END IF;
    IF p_pk IS NOT NULL AND NOT pg_temp.has_col(p_table, p_pk) THEN
        RAISE NOTICE 'Skip % (no PK col %)', p_table, p_pk;
        RETURN 0;
    END IF;

    SELECT coalesce(array_agg(c.column_name::text ORDER BY c.ordinal_position), ARRAY[]::text[])
    INTO v_cols
    FROM information_schema.columns c
    WHERE c.table_schema = 'public'
      AND c.table_name = p_table
      AND (p_pk IS NULL OR c.column_name <> p_pk)
      AND c.is_generated = 'NEVER'
      AND (c.is_identity = 'NO' OR c.identity_generation = 'BY DEFAULT');

    IF coalesce(array_length(v_cols, 1), 0) = 0 THEN
        RAISE NOTICE 'Skip % (no insertable columns)', p_table;
        RETURN 0;
    END IF;

    FOREACH v_col IN ARRAY v_cols LOOP
        IF v_insert_cols <> '' THEN
            v_insert_cols := v_insert_cols || ', ';
            v_select_exprs := v_select_exprs || ', ';
        END IF;
        v_insert_cols := v_insert_cols || quote_ident(v_col);

        IF v_col = p_company_col THEN
            v_expr := p_dst::text;
        ELSIF p_null_self_fk IS NOT NULL AND v_col = p_null_self_fk THEN
            v_expr := 'NULL';
        ELSE
            v_found := FALSE;
            IF p_fk_maps IS NOT NULL THEN
                FOR v_i IN 1 .. coalesce(array_length(p_fk_maps, 1), 0) LOOP
                    IF p_fk_maps[v_i][1] = v_col THEN
                        -- First matching map entity wins via nested COALESCE built in loop order:
                        -- use subquery that tries this entity; later entries for same col need
                        -- separate handling — we build COALESCE chain for same column.
                        IF NOT v_found THEN
                            v_expr := format(
                                'COALESCE((SELECT g.new_id FROM map_generic g WHERE g.entity = %L AND g.old_id = s.%I)',
                                p_fk_maps[v_i][2], v_col
                            );
                            v_found := TRUE;
                        ELSE
                            v_expr := v_expr || format(
                                ', (SELECT g.new_id FROM map_generic g WHERE g.entity = %L AND g.old_id = s.%I)',
                                p_fk_maps[v_i][2], v_col
                            );
                        END IF;
                    END IF;
                END LOOP;
                IF v_found THEN
                    v_expr := v_expr || format(', s.%I)', v_col);
                END IF;
            END IF;
            IF NOT v_found THEN
                v_expr := format('s.%I', v_col);
            END IF;
        END IF;
        v_select_exprs := v_select_exprs || v_expr;
    END LOOP;

    IF p_pk IS NOT NULL AND p_map_entity IS NOT NULL THEN
        v_sql := format($f$
            WITH ordered_src AS (
                SELECT s.*, row_number() OVER (ORDER BY s.%I) AS rn
                FROM %I s
                WHERE s.%I = $1
            ),
            ins AS (
                INSERT INTO %I (%s)
                SELECT %s
                FROM ordered_src s
                ORDER BY s.rn
                RETURNING %I AS new_id
            ),
            neu AS (
                SELECT new_id, row_number() OVER (ORDER BY new_id) AS rn FROM ins
            )
            INSERT INTO map_generic(entity, old_id, new_id)
            SELECT $2, os.%I, neu.new_id
            FROM ordered_src os
            JOIN neu ON neu.rn = os.rn
        $f$, p_pk, p_table, p_company_col,
             p_table, v_insert_cols, v_select_exprs, p_pk,
             p_pk);
        EXECUTE v_sql USING p_src, p_map_entity;
        GET DIAGNOSTICS v_count = ROW_COUNT;
        RAISE NOTICE 'Cloned % map(%) from %', v_count, p_map_entity, p_table;
    ELSE
        v_sql := format(
            'INSERT INTO %I (%s) SELECT %s FROM %I s WHERE s.%I = $1',
            p_table, v_insert_cols, v_select_exprs, p_table, p_company_col
        );
        EXECUTE v_sql USING p_src;
        GET DIAGNOSTICS v_count = ROW_COUNT;
        RAISE NOTICE 'Cloned % rows from %', v_count, p_table;
    END IF;

    RETURN v_count;
EXCEPTION WHEN OTHERS THEN
    RAISE WARNING 'ERROR cloning %: %', p_table, SQLERRM;
    RETURN 0;
END;
$$;

CREATE OR REPLACE FUNCTION pg_temp.patch_self_fk(
    p_table text,
    p_pk text,
    p_self_fk text,
    p_map_entity text,
    p_dst int,
    p_company_col text DEFAULT 'company_id'
)
RETURNS void
LANGUAGE plpgsql
AS $$
DECLARE
    v_count bigint;
BEGIN
    IF NOT pg_temp.table_exists(p_table) THEN RETURN; END IF;
    IF NOT pg_temp.has_col(p_table, p_self_fk) THEN RETURN; END IF;

    EXECUTE format($f$
        UPDATE %I d
        SET %I = mpar.new_id
        FROM map_generic mchild
        JOIN %I s_old ON s_old.%I = mchild.old_id
        JOIN map_generic mpar ON mpar.entity = $1 AND mpar.old_id = s_old.%I
        WHERE mchild.entity = $1
          AND d.%I = mchild.new_id
          AND d.%I = $2
          AND s_old.%I IS NOT NULL
    $f$, p_table, p_self_fk, p_table, p_pk, p_self_fk, p_pk, p_company_col, p_self_fk)
    USING p_map_entity, p_dst;

    GET DIAGNOSTICS v_count = ROW_COUNT;
    RAISE NOTICE 'Patched self-FK %.% → % rows', p_table, p_self_fk, v_count;
END;
$$;

-- =============================================================================
-- STEP A — Preflight counts
-- =============================================================================
DO $$
DECLARE
    r RECORD;
    v_src INT := 10001;
    v_dst INT := 10004;
    v_src_cnt BIGINT;
    v_dst_cnt BIGINT;
BEGIN
    RAISE NOTICE '--- PREFLIGHT COUNTS ---';
    FOR r IN SELECT * FROM pg_temp.company_tables() ORDER BY 1 LOOP
        EXECUTE format('SELECT COUNT(*) FROM %I WHERE %I = $1', r.table_name, r.company_col)
            INTO v_src_cnt USING v_src;
        EXECUTE format('SELECT COUNT(*) FROM %I WHERE %I = $1', r.table_name, r.company_col)
            INTO v_dst_cnt USING v_dst;
        IF v_src_cnt > 0 OR v_dst_cnt > 0 THEN
            RAISE NOTICE '% | src=% | dst=%', rpad(r.table_name, 40), v_src_cnt, v_dst_cnt;
        END IF;
    END LOOP;
END $$;

-- =============================================================================
-- STEP B — Wipe destination 10004
-- =============================================================================
DO $$
DECLARE
    v_dst INT := 10004;
    r RECORD;
    t TEXT;
    v_deleted BIGINT;
    v_known TEXT[] := ARRAY[
        'mob_order_detail','mob_order','tab_pay_rec','tab_sal_pur_tran','tab_sale_pur','tab_jv',
        'tab_role_permission','tab_user_role',
        'tab_party_reporting_tags','tab_party_contact_persons','tab_party_bank_details','tab_party_address',
        'tab_tax_group_detail','tab_currency_adjustment','tab_notifications','tab_system_log',
        'tab_ledger_settings','tab_product_master','tab_product_group_master','tab_party_master',
        'tab_party_type_master','tab_user_master','tab_role_master','tab_vehicle_master',
        'tab_transporter_master','tab_salesperson_master','tab_zone_master','tab_godown_master',
        'tab_location','tab_warehouse_master','tab_branch_master','tab_fiscal_year_master',
        'tab_unit_master','tab_hsn_master','tab_sac_master','tab_tax_master','tab_tax_group_master',
        'tab_tax_exemption_master','tab_tax_preference_master','tab_tcs_master','tab_tds_master',
        'tab_payment_mode_master','tab_payment_term_master','tab_outstanding_term_color_rules',
        'tab_currency_master','tab_cost_center_master','tab_project_master','tab_address_type_master',
        'tab_bank_master','tab_employee','tab_account_group_master','tab_voucher_type_master',
        'tab_account_master','tab_add_price'
    ];
BEGIN
    RAISE NOTICE '--- WIPE DESTINATION = % ---', v_dst;
    PERFORM set_config('session_replication_role', 'replica', true);

    FOREACH t IN ARRAY v_known LOOP
        IF to_regclass('public.' || t) IS NULL THEN CONTINUE; END IF;

        IF t = 'mob_order_detail' AND to_regclass('public.mob_order') IS NOT NULL
           AND NOT EXISTS (
               SELECT 1 FROM information_schema.columns
               WHERE table_schema='public' AND table_name='mob_order_detail' AND column_name='company_id'
           ) THEN
            EXECUTE $q$
                DELETE FROM mob_order_detail d
                USING mob_order o
                WHERE d.mob_order_id = o.mob_order_id AND o.company_id = $1
            $q$ USING v_dst;
            GET DIAGNOSTICS v_deleted = ROW_COUNT;
            RAISE NOTICE 'Deleted % from mob_order_detail', v_deleted;
        ELSIF t = 'tab_jv' AND EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema='public' AND table_name='tab_jv' AND column_name='jv_comp_code'
        ) THEN
            EXECUTE 'DELETE FROM tab_jv WHERE jv_comp_code = $1' USING v_dst;
            GET DIAGNOSTICS v_deleted = ROW_COUNT;
            RAISE NOTICE 'Deleted % from tab_jv', v_deleted;
        ELSIF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema='public' AND table_name=t AND column_name='company_id'
        ) THEN
            EXECUTE format('DELETE FROM %I WHERE company_id = $1', t) USING v_dst;
            GET DIAGNOSTICS v_deleted = ROW_COUNT;
            IF v_deleted > 0 THEN
                RAISE NOTICE 'Deleted % from %', v_deleted, t;
            END IF;
        END IF;
    END LOOP;

    FOR r IN
        SELECT ct.table_name, ct.company_col
        FROM pg_temp.company_tables() ct
        WHERE NOT (ct.table_name = ANY (v_known))
    LOOP
        BEGIN
            EXECUTE format('DELETE FROM %I WHERE %I = $1', r.table_name, r.company_col) USING v_dst;
            GET DIAGNOSTICS v_deleted = ROW_COUNT;
            IF v_deleted > 0 THEN
                RAISE NOTICE 'Deleted % from % (dynamic)', v_deleted, r.table_name;
            END IF;
        EXCEPTION WHEN OTHERS THEN
            RAISE NOTICE 'Wipe skip %: %', r.table_name, SQLERRM;
        END;
    END LOOP;

    PERFORM set_config('session_replication_role', 'origin', true);
END $$;

-- =============================================================================
-- STEP C/D — Clone masters / parties / products / users / ledger
-- =============================================================================
DO $$
DECLARE
    v_src INT := 10001;
    v_dst INT := 10004;
BEGIN
    RAISE NOTICE '--- CLONE CORE MASTERS ---';

    PERFORM pg_temp.clone_table('tab_unit_master', 'unit_id', v_src, v_dst, 'unit');
    PERFORM pg_temp.clone_table('tab_hsn_master', 'hsn_id', v_src, v_dst, 'hsn');
    PERFORM pg_temp.clone_table('tab_sac_master', 'sac_id', v_src, v_dst, 'sac');
    PERFORM pg_temp.clone_table('tab_tax_master', 'tax_id', v_src, v_dst, 'tax');
    PERFORM pg_temp.clone_table('tab_tax_group_master', 'tax_group_id', v_src, v_dst, 'tax_group');
    PERFORM pg_temp.clone_table('tab_tax_exemption_master', 'tax_exemption_id', v_src, v_dst, 'tax_exemption');
    PERFORM pg_temp.clone_table('tab_tax_preference_master', 'tax_preference_id', v_src, v_dst, 'tax_preference');
    PERFORM pg_temp.clone_table('tab_tcs_master', 'tcs_id', v_src, v_dst, 'tcs');
    PERFORM pg_temp.clone_table('tab_tds_master', 'tds_id', v_src, v_dst, 'tds');
    PERFORM pg_temp.clone_table('tab_currency_master', 'currency_id', v_src, v_dst, 'currency');
    PERFORM pg_temp.clone_table('tab_cost_center_master', 'cost_center_id', v_src, v_dst, 'cost_center');
    PERFORM pg_temp.clone_table('tab_fiscal_year_master', 'fiscal_year_id', v_src, v_dst, 'fiscal_year');
    PERFORM pg_temp.clone_table('tab_branch_master', 'branch_id', v_src, v_dst, 'branch');
    PERFORM pg_temp.clone_table('tab_zone_master', 'zone_id', v_src, v_dst, 'zone');
    PERFORM pg_temp.clone_table('tab_party_type_master', 'party_type_id', v_src, v_dst, 'party_type');
    PERFORM pg_temp.clone_table('tab_payment_term_master', 'term_id', v_src, v_dst, 'payment_term');

    IF pg_temp.has_col('tab_payment_mode_master', 'payment_mode_id_internal') THEN
        PERFORM pg_temp.clone_table('tab_payment_mode_master', 'payment_mode_id_internal', v_src, v_dst, 'payment_mode');
    ELSIF pg_temp.has_col('tab_payment_mode_master', 'payment_mode_id') THEN
        PERFORM pg_temp.clone_table('tab_payment_mode_master', 'payment_mode_id', v_src, v_dst, 'payment_mode');
    END IF;

    PERFORM pg_temp.clone_table('tab_salesperson_master', 'salesperson_id', v_src, v_dst, 'salesperson');
    PERFORM pg_temp.clone_table('tab_transporter_master', 'transporter_id', v_src, v_dst, 'transporter');
    PERFORM pg_temp.clone_table('tab_project_master', 'project_id', v_src, v_dst, 'project');
    PERFORM pg_temp.clone_table('tab_address_type_master', 'address_type_id', v_src, v_dst, 'address_type');
    PERFORM pg_temp.clone_table('tab_bank_master', 'bank_master_id', v_src, v_dst, 'bank_master');
    PERFORM pg_temp.clone_table('tab_employee', 'employee_id', v_src, v_dst, 'employee');
    PERFORM pg_temp.clone_table('tab_outstanding_term_color_rules', 'rule_id', v_src, v_dst, 'term_color');
    PERFORM pg_temp.clone_table('tab_voucher_type_master', 'v_type_id', v_src, v_dst, 'voucher_type');

    PERFORM pg_temp.clone_table(
        'tab_warehouse_master', 'warehouse_id', v_src, v_dst, 'warehouse', 'company_id',
        ARRAY[ARRAY['branch_id','branch']]
    );
    PERFORM pg_temp.clone_table(
        'tab_godown_master', 'godown_id', v_src, v_dst, 'godown', 'company_id',
        ARRAY[ARRAY['warehouse_id','warehouse']]
    );
    PERFORM pg_temp.clone_table(
        'tab_location', 'location_id', v_src, v_dst, 'location', 'company_id',
        ARRAY[ARRAY['branch_id','branch'], ARRAY['warehouse_id','warehouse']]
    );
    PERFORM pg_temp.clone_table(
        'tab_currency_adjustment', 'adjustment_id', v_src, v_dst, 'currency_adj', 'company_id',
        ARRAY[ARRAY['currency_id','currency']]
    );
    PERFORM pg_temp.clone_table(
        'tab_tax_group_detail', 'tax_group_detail_id', v_src, v_dst, 'tax_group_detail', 'company_id',
        ARRAY[ARRAY['tax_group_id','tax_group'], ARRAY['tax_id','tax']]
    );
    PERFORM pg_temp.clone_table(
        'tab_vehicle_master', 'vehicle_id', v_src, v_dst, 'vehicle', 'company_id',
        ARRAY[ARRAY['transporter_id','transporter']]
    );

    PERFORM pg_temp.clone_table(
        'tab_account_group_master', 'group_id', v_src, v_dst, 'account_group', 'company_id',
        NULL, 'parent_group_id'
    );
    PERFORM pg_temp.patch_self_fk('tab_account_group_master', 'group_id', 'parent_group_id', 'account_group', v_dst);

    PERFORM pg_temp.clone_table(
        'tab_account_master', 'account_id', v_src, v_dst, 'account', 'company_id',
        NULL, 'parent_account_id'
    );
    PERFORM pg_temp.patch_self_fk('tab_account_master', 'account_id', 'parent_account_id', 'account', v_dst);

    PERFORM pg_temp.clone_table(
        'tab_party_master', 'party_id', v_src, v_dst, 'party', 'company_id',
        ARRAY[
            ARRAY['party_type_id','party_type'],
            ARRAY['zone_id','zone'],
            ARRAY['currency_id','currency']
        ]
    );

    IF pg_temp.table_exists('tab_party_master')
       AND pg_temp.has_col('tab_party_master', 'accounts_receivable_account')
       AND pg_temp.has_col('tab_party_master', 'accounts_payable_account') THEN
        UPDATE tab_party_master d
        SET accounts_receivable_account = COALESCE(
                (SELECT g.new_id FROM map_generic g WHERE g.entity='party' AND g.old_id = s.accounts_receivable_account),
                (SELECT g.new_id FROM map_generic g WHERE g.entity='account' AND g.old_id = s.accounts_receivable_account),
                d.accounts_receivable_account
            ),
            accounts_payable_account = COALESCE(
                (SELECT g.new_id FROM map_generic g WHERE g.entity='party' AND g.old_id = s.accounts_payable_account),
                (SELECT g.new_id FROM map_generic g WHERE g.entity='account' AND g.old_id = s.accounts_payable_account),
                d.accounts_payable_account
            )
        FROM map_generic m
        JOIN tab_party_master s ON s.party_id = m.old_id AND s.company_id = v_src
        WHERE m.entity = 'party'
          AND d.party_id = m.new_id
          AND d.company_id = v_dst;
    END IF;

    PERFORM pg_temp.clone_table(
        'tab_party_address', 'address_id', v_src, v_dst, 'party_address', 'company_id',
        ARRAY[ARRAY['party_id','party'], ARRAY['address_type_id','address_type']]
    );
    PERFORM pg_temp.clone_table(
        'tab_party_bank_details', 'bank_id', v_src, v_dst, 'party_bank', 'company_id',
        ARRAY[ARRAY['party_id','party']]
    );
    PERFORM pg_temp.clone_table(
        'tab_party_contact_persons', 'contact_person_id', v_src, v_dst, 'party_contact', 'company_id',
        ARRAY[ARRAY['party_id','party']]
    );
    PERFORM pg_temp.clone_table(
        'tab_party_reporting_tags', 'tag_id', v_src, v_dst, 'party_tag', 'company_id',
        ARRAY[ARRAY['party_id','party']]
    );

    PERFORM pg_temp.clone_table(
        'tab_product_group_master', 'product_group_id', v_src, v_dst, 'product_group', 'company_id',
        ARRAY[ARRAY['trading_account_id','party'], ARRAY['trading_account_id','account']]
    );
    PERFORM pg_temp.clone_table(
        'tab_product_master', 'product_id', v_src, v_dst, 'product', 'company_id',
        ARRAY[
            ARRAY['unit_id','unit'],
            ARRAY['hsn_id','hsn'],
            ARRAY['sac_id','sac'],
            ARRAY['tax_preference_id','tax_preference'],
            ARRAY['tax_group_id','tax_group'],
            ARRAY['product_group_id','product_group'],
            ARRAY['product_packing_unit','unit'],
            ARRAY['preferred_vendor_id','party'],
            ARRAY['purchase_effected_account','party'],
            ARRAY['sale_effected_account','party'],
            ARRAY['purchase_effected_account','account'],
            ARRAY['sale_effected_account','account'],
            ARRAY['tax_exemption_id','tax_exemption']
        ]
    );

    PERFORM pg_temp.clone_table('tab_user_master', 'user_id', v_src, v_dst, 'user');
    PERFORM pg_temp.clone_table('tab_role_master', 'role_id', v_src, v_dst, 'role');
    PERFORM pg_temp.clone_table(
        'tab_user_role', NULL, v_src, v_dst, NULL, 'company_id',
        ARRAY[ARRAY['user_id','user'], ARRAY['role_id','role']]
    );
    PERFORM pg_temp.clone_table(
        'tab_role_permission', 'permission_id', v_src, v_dst, 'role_permission', 'company_id',
        ARRAY[ARRAY['role_id','role']]
    );

    PERFORM pg_temp.clone_table(
        'tab_ledger_settings', 'setting_id', v_src, v_dst, 'ledger_settings', 'company_id',
        ARRAY[
            ARRAY['sales_revenue_account_id','party'],
            ARRAY['purchase_expense_account_id','party'],
            ARRAY['cgst_output_account_id','party'],
            ARRAY['sgst_output_account_id','party'],
            ARRAY['igst_output_account_id','party'],
            ARRAY['cgst_input_account_id','party'],
            ARRAY['sgst_input_account_id','party'],
            ARRAY['igst_input_account_id','party'],
            ARRAY['round_off_account_id','party'],
            ARRAY['discount_account_id','party'],
            ARRAY['sales_revenue_account_id','account'],
            ARRAY['purchase_expense_account_id','account'],
            ARRAY['cgst_output_account_id','account'],
            ARRAY['sgst_output_account_id','account'],
            ARRAY['igst_output_account_id','account'],
            ARRAY['cgst_input_account_id','account'],
            ARRAY['sgst_input_account_id','account'],
            ARRAY['igst_input_account_id','account'],
            ARRAY['round_off_account_id','account'],
            ARRAY['discount_account_id','account']
        ]
    );
END $$;

-- =============================================================================
-- STEP E — Clone transactions
-- =============================================================================
DO $$
DECLARE
    v_src INT := 10001;
    v_dst INT := 10004;
BEGIN
    RAISE NOTICE '--- CLONE TRANSACTIONS ---';

    PERFORM pg_temp.clone_table(
        'tab_sale_pur', 'sp_unique_code', v_src, v_dst, 'sale_pur', 'company_id',
        ARRAY[
            ARRAY['party_id','party'],
            ARRAY['salesperson_id','salesperson'],
            ARRAY['transporter_id','transporter'],
            ARRAY['unit_id','unit'],
            ARRAY['bank_id','party'],
            ARRAY['bank_id','bank_master'],
            ARRAY['product_id','product'],
            ARRAY['tax_id','tax'],
            ARRAY['terms_id','payment_term'],
            ARRAY['fiscal_year_id','fiscal_year'],
            ARRAY['branch_id','branch'],
            ARRAY['created_by','user'],
            ARRAY['modified_by','user'],
            ARRAY['deleted_by','user']
        ]
    );

    PERFORM pg_temp.clone_table(
        'tab_sal_pur_tran', 'sp_tran_unique_code', v_src, v_dst, 'sale_pur_tran', 'company_id',
        ARRAY[
            ARRAY['sp_unique_code','sale_pur'],
            ARRAY['product_id','product'],
            ARRAY['sub_unit','unit'],
            ARRAY['unit','unit'],
            ARRAY['fiscal_year_id','fiscal_year'],
            ARRAY['branch_id','branch'],
            ARRAY['created_by','user'],
            ARRAY['modified_by','user'],
            ARRAY['deleted_by','user']
        ]
    );

    PERFORM pg_temp.clone_table(
        'tab_jv', 'jv_code', v_src, v_dst, 'jv', 'jv_comp_code',
        ARRAY[
            ARRAY['dr_account_code','party'],
            ARRAY['cr_account_code','party'],
            ARRAY['dr_account_code','account'],
            ARRAY['cr_account_code','account'],
            ARRAY['v_code','sale_pur'],
            ARRAY['year_code','fiscal_year'],
            ARRAY['branch_code','branch']
        ]
    );

    PERFORM pg_temp.clone_table(
        'tab_pay_rec', 'code', v_src, v_dst, 'pay_rec', 'company_id',
        ARRAY[
            ARRAY['inv_code','sale_pur'],
            ARRAY['jv_code','jv'],
            ARRAY['party_id','party'],
            ARRAY['account_id','party'],
            ARRAY['account_id','account'],
            ARRAY['year_code','fiscal_year'],
            ARRAY['branch_code','branch'],
            ARRAY['user_code','user']
        ]
    );

    PERFORM pg_temp.clone_table(
        'mob_order', 'mob_order_id', v_src, v_dst, 'mob_order', 'company_id',
        ARRAY[
            ARRAY['dock_manager_id','user'],
            ARRAY['fiscal_year_id','fiscal_year'],
            ARRAY['branch_id','branch'],
            ARRAY['sp_unique_code','sale_pur']
        ]
    );

    IF pg_temp.table_exists('mob_order_detail') AND pg_temp.table_exists('mob_order') THEN
        IF pg_temp.has_col('mob_order_detail', 'company_id') THEN
            PERFORM pg_temp.clone_table(
                'mob_order_detail', 'mob_order_detail_id', v_src, v_dst, 'mob_order_detail', 'company_id',
                ARRAY[ARRAY['mob_order_id','mob_order'], ARRAY['product_id','product']]
            );
        ELSE
            BEGIN
                EXECUTE $q$
                    WITH ordered_src AS (
                        SELECT d.mob_order_detail_id, d.mob_order_id, d.product_id, d.quantity,
                               row_number() OVER (ORDER BY d.mob_order_detail_id) AS rn
                        FROM mob_order_detail d
                        JOIN mob_order o ON o.mob_order_id = d.mob_order_id
                        WHERE o.company_id = $1
                    ),
                    ins AS (
                        INSERT INTO mob_order_detail (mob_order_id, product_id, quantity)
                        SELECT
                            (SELECT g.new_id FROM map_generic g WHERE g.entity='mob_order' AND g.old_id = s.mob_order_id),
                            COALESCE((SELECT g.new_id FROM map_generic g WHERE g.entity='product' AND g.old_id = s.product_id), s.product_id),
                            s.quantity
                        FROM ordered_src s
                        ORDER BY s.rn
                        RETURNING mob_order_detail_id AS new_id
                    ),
                    neu AS (
                        SELECT new_id, row_number() OVER (ORDER BY new_id) AS rn FROM ins
                    )
                    INSERT INTO map_generic(entity, old_id, new_id)
                    SELECT 'mob_order_detail', os.mob_order_detail_id, neu.new_id
                    FROM ordered_src os
                    JOIN neu ON neu.rn = os.rn
                $q$ USING v_src;
                RAISE NOTICE 'Cloned mob_order_detail via parent company_id';
            EXCEPTION WHEN OTHERS THEN
                RAISE WARNING 'mob_order_detail clone failed: %', SQLERRM;
            END;
        END IF;
    END IF;
END $$;

-- =============================================================================
-- STEP F — Leftover tables (safe company_id-only rewrite)
-- =============================================================================
DO $$
DECLARE
    v_src INT := 10001;
    v_dst INT := 10004;
    r RECORD;
    v_src_cnt BIGINT;
    v_dst_cnt BIGINT;
    v_handled TEXT[] := ARRAY[
        'tab_unit_master','tab_hsn_master','tab_sac_master','tab_tax_master','tab_tax_group_master',
        'tab_tax_exemption_master','tab_tax_preference_master','tab_tcs_master','tab_tds_master',
        'tab_currency_master','tab_cost_center_master','tab_fiscal_year_master','tab_branch_master',
        'tab_zone_master','tab_party_type_master','tab_payment_term_master','tab_payment_mode_master',
        'tab_salesperson_master','tab_transporter_master','tab_project_master','tab_address_type_master',
        'tab_bank_master','tab_employee','tab_outstanding_term_color_rules','tab_voucher_type_master',
        'tab_warehouse_master','tab_godown_master','tab_location','tab_currency_adjustment',
        'tab_tax_group_detail','tab_vehicle_master','tab_account_group_master','tab_account_master',
        'tab_party_master','tab_party_address','tab_party_bank_details','tab_party_contact_persons',
        'tab_party_reporting_tags','tab_product_group_master','tab_product_master',
        'tab_user_master','tab_role_master','tab_user_role','tab_role_permission','tab_ledger_settings',
        'tab_sale_pur','tab_sal_pur_tran','tab_jv','tab_pay_rec','mob_order','mob_order_detail',
        'tabs','tab_company_master','tab_add_price'
    ];
    v_risky TEXT[] := ARRAY[
        'party_id','product_id','product_group_id','unit_id','hsn_id','sac_id','tax_id','tax_group_id',
        'branch_id','warehouse_id','godown_id','location_id','fiscal_year_id','user_id','role_id',
        'salesperson_id','transporter_id','zone_id','party_type_id','term_id','vehicle_id',
        'sp_unique_code','jv_code','account_id','group_id','parent_group_id','parent_account_id',
        'dr_account_code','cr_account_code','v_code','inv_code','mob_order_id'
    ];
    v_has_risky boolean;
    v_pk text;
    v_cnt bigint;
BEGIN
    RAISE NOTICE '--- LEFTOVER TABLES ---';
    FOR r IN SELECT * FROM pg_temp.company_tables() ORDER BY 1 LOOP
        IF r.table_name = ANY (v_handled) THEN CONTINUE; END IF;

        EXECUTE format('SELECT COUNT(*) FROM %I WHERE %I = $1', r.table_name, r.company_col)
            INTO v_src_cnt USING v_src;
        EXECUTE format('SELECT COUNT(*) FROM %I WHERE %I = $1', r.table_name, r.company_col)
            INTO v_dst_cnt USING v_dst;

        IF v_src_cnt = 0 THEN CONTINUE; END IF;
        IF v_dst_cnt > 0 THEN
            RAISE NOTICE 'Leftover % already has % dst rows — skip', r.table_name, v_dst_cnt;
            CONTINUE;
        END IF;

        SELECT EXISTS (
            SELECT 1 FROM information_schema.columns c
            WHERE c.table_schema='public' AND c.table_name=r.table_name
              AND c.column_name = ANY (v_risky)
        ) INTO v_has_risky;

        IF v_has_risky THEN
            RAISE NOTICE 'SKIP leftover % (FK-like cols; extend maps). src=%', r.table_name, v_src_cnt;
            CONTINUE;
        END IF;

        SELECT a.attname INTO v_pk
        FROM pg_index i
        JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
        WHERE i.indrelid = ('public.' || r.table_name)::regclass
          AND i.indisprimary
        LIMIT 1;

        v_cnt := pg_temp.clone_table(r.table_name, v_pk, v_src, v_dst, NULL, r.company_col, NULL);
        RAISE NOTICE 'Leftover cloned % rows=%', r.table_name, v_cnt;
    END LOOP;
END $$;

-- =============================================================================
-- STEP G — Reset sequences
-- =============================================================================
DO $$
DECLARE
    r RECORD;
    v_seq text;
    v_max bigint;
BEGIN
    RAISE NOTICE '--- RESET SEQUENCES ---';
    FOR r IN
        SELECT c.table_name, c.column_name
        FROM information_schema.columns c
        JOIN information_schema.tables t
          ON t.table_schema=c.table_schema AND t.table_name=c.table_name
        WHERE c.table_schema='public'
          AND t.table_type='BASE TABLE'
          AND c.column_default LIKE 'nextval%'
    LOOP
        BEGIN
            v_seq := pg_get_serial_sequence('public.' || r.table_name, r.column_name);
            IF v_seq IS NULL THEN CONTINUE; END IF;
            EXECUTE format('SELECT COALESCE(MAX(%I), 1) FROM %I', r.column_name, r.table_name) INTO v_max;
            PERFORM setval(v_seq, v_max, true);
        EXCEPTION WHEN OTHERS THEN
            NULL;
        END;
    END LOOP;
END $$;

-- =============================================================================
-- STEP H — Verification
-- =============================================================================
DO $$
DECLARE
    r RECORD;
    v_src INT := 10001;
    v_dst INT := 10004;
    v_src_cnt BIGINT;
    v_dst_cnt BIGINT;
    v_mismatch INT := 0;
BEGIN
    RAISE NOTICE '--- VERIFICATION ---';
    FOR r IN SELECT * FROM pg_temp.company_tables() ORDER BY 1 LOOP
        EXECUTE format('SELECT COUNT(*) FROM %I WHERE %I = $1', r.table_name, r.company_col)
            INTO v_src_cnt USING v_src;
        EXECUTE format('SELECT COUNT(*) FROM %I WHERE %I = $1', r.table_name, r.company_col)
            INTO v_dst_cnt USING v_dst;
        IF v_src_cnt = 0 AND v_dst_cnt = 0 THEN CONTINUE; END IF;
        IF v_src_cnt <> v_dst_cnt THEN
            v_mismatch := v_mismatch + 1;
            RAISE NOTICE 'MISMATCH % | src=% dst=%', rpad(r.table_name, 40), v_src_cnt, v_dst_cnt;
        ELSE
            RAISE NOTICE 'OK       % | %', rpad(r.table_name, 40), v_src_cnt;
        END IF;
    END LOOP;
    RAISE NOTICE 'Mismatch tables: %', v_mismatch;
    RAISE NOTICE 'Map rows stored: %', (SELECT COUNT(*) FROM map_generic);
END $$;

SELECT 'party' AS entity,
       (SELECT COUNT(*) FROM tab_party_master WHERE company_id = 10001) AS src_10001,
       (SELECT COUNT(*) FROM tab_party_master WHERE company_id = 10004) AS dst_10004
UNION ALL
SELECT 'product',
       (SELECT COUNT(*) FROM tab_product_master WHERE company_id = 10001),
       (SELECT COUNT(*) FROM tab_product_master WHERE company_id = 10004)
UNION ALL
SELECT 'sale_pur',
       (SELECT COUNT(*) FROM tab_sale_pur WHERE company_id = 10001),
       (SELECT COUNT(*) FROM tab_sale_pur WHERE company_id = 10004)
UNION ALL
SELECT 'jv',
       (SELECT COUNT(*) FROM tab_jv WHERE jv_comp_code = 10001),
       (SELECT COUNT(*) FROM tab_jv WHERE jv_comp_code = 10004)
UNION ALL
SELECT 'user',
       (SELECT COUNT(*) FROM tab_user_master WHERE company_id = 10001),
       (SELECT COUNT(*) FROM tab_user_master WHERE company_id = 10004)
UNION ALL
SELECT 'role_permission',
       (SELECT COUNT(*) FROM tab_role_permission WHERE company_id = 10001),
       (SELECT COUNT(*) FROM tab_role_permission WHERE company_id = 10004);

COMMIT;

-- Done. On failure use ROLLBACK instead.
-- Login company 10004 with users copied from 10001.
