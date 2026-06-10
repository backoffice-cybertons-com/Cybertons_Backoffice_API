-- Dashboard: track when new products were added
-- Safe minimal change — no stored procedure changes required.
--
-- Why this works with sp_master_universal:
--   Universal INSERT builds column list dynamically but EXCLUDES created_on / created_at
--   (see sp_Master_Universal_Postgres.sql INSERT block).
--   New rows therefore get created_on from this column DEFAULT automatically.
--
-- Why this works with fn_product_master / sp_product_create:
--   Those INSERT statements also omit created_on, so the same DEFAULT applies.
--
-- Existing products are not modified (created_on stays NULL for old rows).

ALTER TABLE public.tab_product_master
    ADD COLUMN IF NOT EXISTS created_on TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP;
