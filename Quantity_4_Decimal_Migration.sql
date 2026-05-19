-- Run this once in each tenant database that needs 4-decimal quantity support.
-- It changes existing quantity columns only; it does not add new columns.

ALTER TABLE IF EXISTS public.tab_product_master
    ALTER COLUMN opening_quantity TYPE numeric(18,4) USING opening_quantity::numeric(18,4),
    ALTER COLUMN min_quantity TYPE numeric(18,4) USING min_quantity::numeric(18,4),
    ALTER COLUMN max_quantity TYPE numeric(18,4) USING max_quantity::numeric(18,4),
    ALTER COLUMN re_order_quantity TYPE numeric(18,4) USING re_order_quantity::numeric(18,4);

DO $$
DECLARE
    active_sale_pur_view_def text;
    active_sale_pur_view_owner text;
    active_sale_pur_tran_view_def text;
    active_sale_pur_tran_view_owner text;
BEGIN
    IF to_regclass('public.vw_h_active_sale_pur') IS NOT NULL THEN
        SELECT
            pg_get_viewdef('public.vw_h_active_sale_pur'::regclass, true),
            pg_get_userbyid(c.relowner)
        INTO active_sale_pur_view_def, active_sale_pur_view_owner
        FROM pg_class c
        WHERE c.oid = 'public.vw_h_active_sale_pur'::regclass;

        EXECUTE 'DROP VIEW public.vw_h_active_sale_pur';
    END IF;

    IF to_regclass('public.vw_h_active_sale_pur_tran') IS NOT NULL THEN
        SELECT
            pg_get_viewdef('public.vw_h_active_sale_pur_tran'::regclass, true),
            pg_get_userbyid(c.relowner)
        INTO active_sale_pur_tran_view_def, active_sale_pur_tran_view_owner
        FROM pg_class c
        WHERE c.oid = 'public.vw_h_active_sale_pur_tran'::regclass;

        EXECUTE 'DROP VIEW public.vw_h_active_sale_pur_tran';
    END IF;

    ALTER TABLE IF EXISTS public.tab_sale_pur
        ALTER COLUMN total_quantity TYPE numeric(18,4) USING total_quantity::numeric(18,4),
        ALTER COLUMN total_quantity SET DEFAULT 0.0000;

    ALTER TABLE IF EXISTS public.tab_sal_pur_tran
        ALTER COLUMN quantity TYPE numeric(18,4) USING quantity::numeric(18,4),
        ALTER COLUMN quantity SET DEFAULT 0.0000,
        ALTER COLUMN sub_unit_quantity TYPE numeric(18,4) USING sub_unit_quantity::numeric(18,4),
        ALTER COLUMN sub_unit_quantity SET DEFAULT 0.0000;

    IF active_sale_pur_view_def IS NOT NULL THEN
        EXECUTE 'CREATE VIEW public.vw_h_active_sale_pur AS ' || active_sale_pur_view_def;

        IF active_sale_pur_view_owner IS NOT NULL THEN
            EXECUTE format('ALTER VIEW public.vw_h_active_sale_pur OWNER TO %I', active_sale_pur_view_owner);
        END IF;
    END IF;

    IF active_sale_pur_tran_view_def IS NOT NULL THEN
        EXECUTE 'CREATE VIEW public.vw_h_active_sale_pur_tran AS ' || active_sale_pur_tran_view_def;

        IF active_sale_pur_tran_view_owner IS NOT NULL THEN
            EXECUTE format('ALTER VIEW public.vw_h_active_sale_pur_tran OWNER TO %I', active_sale_pur_tran_view_owner);
        END IF;
    END IF;
END $$;
