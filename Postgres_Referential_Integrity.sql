-- CLEAN POSTGRES REFERENTIAL INTEGRITY SCRIPT
-- RUN THIS SCRIPT TO ENFORCE DATA INTEGRITY

-- 1. Remove "Orphan" Data first (Data that would block adding constraints)
DELETE FROM tab_warehouse_master WHERE branch_id NOT IN (SELECT branch_id FROM tab_branch_master) AND branch_id IS NOT NULL;
DELETE FROM tab_godown_master WHERE warehouse_id NOT IN (SELECT warehouse_id FROM tab_warehouse_master) AND warehouse_id IS NOT NULL;
DELETE FROM tab_product_master WHERE unit_id NOT IN (SELECT unit_id FROM tab_unit_master) AND unit_id IS NOT NULL;
DELETE FROM tab_product_master WHERE hsn_id NOT IN (SELECT hsn_id FROM tab_hsn_master) AND hsn_id IS NOT NULL;
DELETE FROM tab_product_master WHERE tax_group_id NOT IN (SELECT tax_group_id FROM tab_tax_group_master) AND tax_group_id IS NOT NULL;

-- 2. Add Constraints (Master Relationships)
ALTER TABLE tab_warehouse_master DROP CONSTRAINT IF EXISTS fk_warehouse_branch;
ALTER TABLE tab_warehouse_master ADD CONSTRAINT fk_warehouse_branch FOREIGN KEY (branch_id) REFERENCES tab_branch_master(branch_id);

ALTER TABLE tab_godown_master DROP CONSTRAINT IF EXISTS fk_godown_warehouse;
ALTER TABLE tab_godown_master ADD CONSTRAINT fk_godown_warehouse FOREIGN KEY (warehouse_id) REFERENCES tab_warehouse_master(warehouse_id);

ALTER TABLE tab_location DROP CONSTRAINT IF EXISTS fk_location_branch;
ALTER TABLE tab_location ADD CONSTRAINT fk_location_branch FOREIGN KEY (branch_id) REFERENCES tab_branch_master(branch_id);

ALTER TABLE tab_location DROP CONSTRAINT IF EXISTS fk_location_warehouse;
ALTER TABLE tab_location ADD CONSTRAINT fk_location_warehouse FOREIGN KEY (warehouse_id) REFERENCES tab_warehouse_master(warehouse_id);

-- 3. Add Constraints (Product & Party)
ALTER TABLE tab_product_master DROP CONSTRAINT IF EXISTS fk_product_unit;
ALTER TABLE tab_product_master ADD CONSTRAINT fk_product_unit FOREIGN KEY (unit_id) REFERENCES tab_unit_master(unit_id);

ALTER TABLE tab_product_master DROP CONSTRAINT IF EXISTS fk_product_hsn;
ALTER TABLE tab_product_master ADD CONSTRAINT fk_product_hsn FOREIGN KEY (hsn_id) REFERENCES tab_hsn_master(hsn_id);

ALTER TABLE tab_party_master DROP CONSTRAINT IF EXISTS fk_party_currency;
ALTER TABLE tab_party_master ADD CONSTRAINT fk_party_currency FOREIGN KEY (currency_id) REFERENCES tab_currency_master(currency_id);

ALTER TABLE tab_party_address DROP CONSTRAINT IF EXISTS fk_address_party;
ALTER TABLE tab_party_address ADD CONSTRAINT fk_address_party FOREIGN KEY (party_id) REFERENCES tab_party_master(party_id);

-- 4. Verification Query (Run this to see if it worked)
-- SELECT conname, contype FROM pg_constraint WHERE conname LIKE 'fk_%';
