# Register / Seed a New Company (Multi-Tenant)

This guide registers a new logical company (example: **company_id = 10004**) and seeds its tenant data.

There is **no API endpoint** that creates companies. Provisioning is manual SQL:

1. Insert routing row in **CompanyMaster** (`companymaster.tab_company_master`)
2. Seed data in the **tenant DB** pointed to by that connection string (e.g. `softaccdb_1_3107`)

---

## Architecture (quick)

| Database | From | Purpose |
|----------|------|---------|
| `companymaster` | `appsettings.json` → `ConnectionStrings:DefaultConnection` | Company registry + Npgsql connection string per `company_id` |
| `softaccdb_*` (tenant shard) | Value in `tab_company_master.connection_string` | Shared SoftAcc data; rows filtered by `company_id` |

Login flow:

1. App looks up `company_id` in `companymaster`
2. Switches to that company’s `connection_string`
3. Validates user in tenant `tab_user_master` (SHA256 Base64 via `PasswordHelper`)

**DB naming in this repo:** `softaccdb_*` (not `softbook_*`).

---

## What we seed vs what we skip

Live company **10001** does **not** need seeded `tab_account_group_master` / `tab_voucher_type_master` for normal app use (sales/purchase/GST use `tab_party_master` + `tab_ledger_settings`). Those masters exist as optional CRUD screens only.

| Seed | Required for new company? |
|------|---------------------------|
| `tab_company_master` (routing) | **Yes** |
| Default accounts in `tab_party_master` + `tab_ledger_settings` | **Yes** (GST / sales / purchase mapping) |
| Fiscal year + Admin user/role + permissions | **Yes** (login + menu) |
| `tab_account_group_master` | **No** — skipped (not used by 10001 flow) |
| `tab_voucher_type_master` | **No** — skipped (not used by 10001 flow) |

The old `sp_seed_company_accounting_data` procedure *does* insert groups/voucher types; our register scripts **do not** call that full procedure anymore.

## Discrepancies fixed vs your pasted scripts

| Issue | Your script | Correct |
|-------|-------------|---------|
| Tenant DB name in comments | `softbook_1_3107` | Use actual name, e.g. **`softaccdb_1_3107`** |
| Comments say company **10001** | Mixed with `10004` | Use **10004** everywhere for this company |
| FY comment says 2025-26 | Dates are `2026-04-01` → `2027-03-31` / label `2026-27` | Comment must match dates (or change dates to FY 2025-26) |
| Seeding account groups / voucher types | Included in `sp_seed_...` | **Skipped** — not required for existing 10001-style app |
| Call seed procedure inside `DO $$` | `PERFORM sp_seed_...` | Not used; inline account + ledger seed instead |
| `tabs` are shared menu | Permissions select **all** active tabs | OK if menu is shared on the shard; do **not** expect per-company `tabs` rows for 10004 |
| First step missing | Started at accounting seed | **Must** insert `tab_company_master` first or login cannot route |

---

## Prerequisites

- [ ] Tenant DB exists and schema is applied (e.g. `softaccdb_1_3107`)
- [ ] Procedure `sp_seed_company_accounting_data` exists on tenant DB (run `sp_seed_company_accounting_data.sql` once if missing)
- [ ] Shared `tabs` menu already present on that tenant DB
- [ ] Choose unique `company_id` (example: `10004`)
- [ ] Know Host / user / password for the tenant shard (match your environment)

---

## STEP 0 — Variables (change once)

| Variable | Example value |
|----------|----------------|
| `company_id` | `10004` |
| Company display name | `Hetak Demo Co 10004` |
| Tenant database | `softaccdb_1_3107` |
| Admin login | `Admin` / `Admin@123` |
| Admin email | `admin@company.com` |

Password hash for `Admin@123` (SHA256 → Base64, same as `PasswordHelper`):

```text
6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=
```

---

## STEP 1 — CompanyMaster: insert company + connection string

**Connect to:** `companymaster`  
(from appsettings: `Host=192.168.1.207;Database=companymaster;...`)

```sql
-- =========================================================
-- STEP 1: Register company in CompanyMaster (routing)
-- Database: companymaster
-- =========================================================

INSERT INTO tab_company_master (
    company_id,
    company_name,
    connection_string,
    is_enabled,
    priority
)
VALUES (
    10004,
    'Hetak Demo Co 10004',   -- <<< change display name
    'Host=192.168.1.207;Database=softaccdb_1_3107;Username=postgres;Password=Soft123',
    TRUE,
    0
)
ON CONFLICT (company_id) DO UPDATE
SET
    company_name      = EXCLUDED.company_name,
    connection_string = EXCLUDED.connection_string,
    is_enabled        = TRUE;

-- Verify
SELECT company_id, company_name, connection_string, is_enabled, priority, created_at
FROM tab_company_master
WHERE company_id = 10004;
```

Notes:

- Omit `code` — it is `SERIAL PRIMARY KEY` and auto-generates.
- `connection_string` must be a full **Npgsql** string pointing at the tenant shard where you will seed rows for `company_id = 10004`.
- Multiple companies may share one tenant DB; isolation is by `company_id` on every tenant table.

---

## STEP 2 — Tenant: ensure ledger mapping columns exist

**Connect to:** tenant DB from Step 1 (`softaccdb_1_3107`)

```sql
-- =========================================================
-- STEP 2: Ensure tab_ledger_settings has account mapping columns
-- Database: softaccdb_1_3107 (tenant)
-- =========================================================

ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS sales_revenue_account_id INT;
ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS purchase_expense_account_id INT;
ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS cgst_output_account_id INT;
ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS sgst_output_account_id INT;
ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS igst_output_account_id INT;
ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS cgst_input_account_id INT;
ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS sgst_input_account_id INT;
ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS igst_input_account_id INT;
ALTER TABLE tab_ledger_settings ADD COLUMN IF NOT EXISTS round_off_account_id INT;
```

---

## STEP 3 — Tenant: create / refresh seed procedure, then call it

```sql
-- =========================================================
-- STEP 3A: Create/replace accounting seed procedure (once per tenant DB)
-- Source file: SoftBook_Backend_API/sp_seed_company_accounting_data.sql
-- =========================================================

CREATE OR REPLACE PROCEDURE sp_seed_company_accounting_data(p_company_id INT)
LANGUAGE plpgsql
AS $$
DECLARE
    v_asset_grp INT; v_liab_grp INT; v_inc_grp INT; v_exp_grp INT;
    v_curr_asset_grp INT; v_curr_liab_grp INT; v_sales_grp INT; v_pur_grp INT;
    v_acc_sales INT; v_acc_pur INT; v_acc_cgst_in INT; v_acc_sgst_in INT; v_acc_igst_in INT;
    v_acc_cgst_out INT; v_acc_sgst_out INT; v_acc_igst_out INT; v_acc_round INT;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM tab_account_group_master WHERE company_id = p_company_id) THEN
        INSERT INTO tab_account_group_master (company_id, group_name, group_type) VALUES (p_company_id, 'Assets', 'Asset') RETURNING group_id INTO v_asset_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type) VALUES (p_company_id, 'Liabilities', 'Liability') RETURNING group_id INTO v_liab_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type) VALUES (p_company_id, 'Income', 'Income') RETURNING group_id INTO v_inc_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type) VALUES (p_company_id, 'Expense', 'Expense') RETURNING group_id INTO v_exp_grp;

        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Current Assets', 'Asset', v_asset_grp) RETURNING group_id INTO v_curr_asset_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Fixed Assets', 'Asset', v_asset_grp);
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Current Liabilities', 'Liability', v_liab_grp) RETURNING group_id INTO v_curr_liab_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Sales Accounts', 'Income', v_inc_grp) RETURNING group_id INTO v_sales_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Purchase Accounts', 'Expense', v_exp_grp) RETURNING group_id INTO v_pur_grp;
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Direct Expenses', 'Expense', v_exp_grp);
        INSERT INTO tab_account_group_master (company_id, group_name, group_type, parent_group_id) VALUES (p_company_id, 'Indirect Expenses', 'Expense', v_exp_grp);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM tab_voucher_type_master WHERE company_id = p_company_id) THEN
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Sales', 'SLS/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Purchase', 'PUR/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Payment', 'PAY/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Receipt', 'RCT/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Journal', 'JV/', 1);
        INSERT INTO tab_voucher_type_master (company_id, v_type_name, prefix, starting_no) VALUES (p_company_id, 'Contra', 'CON/', 1);
    END IF;

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

-- =========================================================
-- STEP 3B: Seed accounting for company 10004
-- =========================================================
CALL sp_seed_company_accounting_data(10004);

SELECT * FROM tab_ledger_settings WHERE company_id = 10004;
SELECT group_name, group_type FROM tab_account_group_master WHERE company_id = 10004;
SELECT v_type_name, prefix FROM tab_voucher_type_master WHERE company_id = 10004;
SELECT party_id, company_name, party_type, display_name
FROM tab_party_master
WHERE company_id = 10004
  AND company_name IN (
      'Sales Account','Purchase Account','CGST Input','SGST Input','IGST Input',
      'CGST Output','SGST Output','IGST Output','Round Off','Cash Account'
  );
```

Idempotent: re-running skips groups/vouchers/accounts if they already exist for that `company_id`.

---

## STEP 4 — Tenant: fiscal year + Admin user + Admin role

```sql
-- =========================================================
-- STEP 4: Fiscal Year + Admin role/user for company 10004
-- Database: softaccdb_1_3107
-- Login: Admin / Admin@123
-- =========================================================

DO $$
DECLARE
    v_company_id INT := 10004;
    v_role_id    INT;
    v_user_id    INT;
    v_pwd_hash   TEXT := '6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc='; -- Admin@123
BEGIN
    -- A) Fiscal Year (India FY 2026-27) — mark as current
    UPDATE tab_fiscal_year_master SET is_current = FALSE WHERE company_id = v_company_id;

    INSERT INTO tab_fiscal_year_master (company_id, start_date, end_date, is_current, fiscal_year_label)
    SELECT v_company_id, DATE '2026-04-01', DATE '2027-03-31', TRUE, '2026-27'
    WHERE NOT EXISTS (
        SELECT 1 FROM tab_fiscal_year_master
        WHERE company_id = v_company_id
          AND start_date = DATE '2026-04-01'
          AND end_date   = DATE '2027-03-31'
    );

    -- If FY already existed but was not current, promote it
    UPDATE tab_fiscal_year_master
    SET is_current = TRUE, fiscal_year_label = COALESCE(fiscal_year_label, '2026-27')
    WHERE company_id = v_company_id
      AND start_date = DATE '2026-04-01'
      AND end_date   = DATE '2027-03-31';

    -- B) Admin Role
    SELECT role_id INTO v_role_id
    FROM tab_role_master
    WHERE company_id = v_company_id AND LOWER(role_name) = 'admin'
    LIMIT 1;

    IF v_role_id IS NULL THEN
        INSERT INTO tab_role_master (company_id, role_name, is_active, is_deleted)
        VALUES (v_company_id, 'Admin', TRUE, FALSE)
        RETURNING role_id INTO v_role_id;
    END IF;

    -- C) Admin User
    SELECT user_id INTO v_user_id
    FROM tab_user_master
    WHERE company_id = v_company_id AND LOWER(username) = 'admin'
    LIMIT 1;

    IF v_user_id IS NULL THEN
        INSERT INTO tab_user_master (company_id, username, password_hash, full_name, email, phone, is_active)
        VALUES (v_company_id, 'Admin', v_pwd_hash, 'Administrator', 'admin@company.com', NULL, TRUE)
        RETURNING user_id INTO v_user_id;
    ELSE
        UPDATE tab_user_master
        SET password_hash = v_pwd_hash, is_active = TRUE
        WHERE user_id = v_user_id;
    END IF;

    -- D) Link User ↔ Admin Role
    IF NOT EXISTS (
        SELECT 1 FROM tab_user_role
        WHERE company_id = v_company_id AND user_id = v_user_id AND role_id = v_role_id
    ) THEN
        INSERT INTO tab_user_role (user_id, role_id, company_id)
        VALUES (v_user_id, v_role_id, v_company_id);
    END IF;

    RAISE NOTICE 'Seeded company_id=%, user_id=%, role_id=%', v_company_id, v_user_id, v_role_id;
END $$;

-- Verify
SELECT * FROM tab_fiscal_year_master WHERE company_id = 10004;
SELECT user_id, username, email, is_active FROM tab_user_master WHERE company_id = 10004;
SELECT * FROM tab_role_master WHERE company_id = 10004;
SELECT * FROM tab_user_role WHERE company_id = 10004;
```

---

## STEP 5 — Tenant: full Admin permissions on shared menu (`tabs`)

```sql
-- =========================================================
-- STEP 5: Give Admin FULL permission on ALL active tabs (shared menu)
-- Database: softaccdb_1_3107
-- =========================================================

DO $$
DECLARE
    v_company_id INT := 10004;
    v_role_id    INT;
    v_count      INT;
BEGIN
    SELECT role_id INTO v_role_id
    FROM tab_role_master
    WHERE company_id = v_company_id
      AND LOWER(role_name) = 'admin'
      AND COALESCE(is_deleted, FALSE) = FALSE
    LIMIT 1;

    IF v_role_id IS NULL THEN
        RAISE EXCEPTION 'Admin role not found for company_id=%. Run Step 4 first.', v_company_id;
    END IF;

    INSERT INTO tab_role_permission (
        company_id, role_id, tab_id,
        can_view, can_add, can_edit, can_delete, is_active
    )
    SELECT
        v_company_id, v_role_id, t.tab_id,
        TRUE, TRUE, TRUE, TRUE, TRUE
    FROM tabs t
    WHERE COALESCE(t.is_active, TRUE) = TRUE
      AND NOT EXISTS (
          SELECT 1 FROM tab_role_permission p
          WHERE p.company_id = v_company_id
            AND p.role_id = v_role_id
            AND p.tab_id = t.tab_id
      );

    GET DIAGNOSTICS v_count = ROW_COUNT;

    UPDATE tab_role_permission
    SET can_view = TRUE, can_add = TRUE, can_edit = TRUE, can_delete = TRUE, is_active = TRUE
    WHERE company_id = v_company_id AND role_id = v_role_id;

    RAISE NOTICE 'Admin role_id=% company_id=% — inserted % new tab permissions',
        v_role_id, v_company_id, v_count;
END $$;

-- Verify counts should match
SELECT COUNT(*) AS admin_tab_perms
FROM tab_role_permission
WHERE company_id = 10004
  AND role_id = (
      SELECT role_id FROM tab_role_master
      WHERE company_id = 10004 AND LOWER(role_name) = 'admin' LIMIT 1
  );

SELECT COUNT(*) AS total_active_tabs
FROM tabs
WHERE COALESCE(is_active, TRUE) = TRUE;

SELECT p.tab_id, t.tab_name, p.can_view, p.can_add, p.can_edit, p.can_delete
FROM tab_role_permission p
JOIN tabs t ON t.tab_id = p.tab_id
WHERE p.company_id = 10004
  AND p.role_id = (
      SELECT role_id FROM tab_role_master
      WHERE company_id = 10004 AND LOWER(role_name) = 'admin' LIMIT 1
  )
ORDER BY t.parent_id NULLS FIRST, t.sort_order;
```

---

## STEP 6 — App login smoke test

1. Restart API if connection strings were cached (usually not needed for new company).
2. Login with:
   - **Company ID:** `10004`
   - **Username:** `Admin`
   - **Password:** `Admin@123`
3. Confirm sidebar loads (permissions from Step 5).

---

## Run order checklist

| # | Database | Action |
|---|----------|--------|
| 1 | `companymaster` | `INSERT` into `tab_company_master` (connection → `softaccdb_1_3107`) |
| 2 | `softaccdb_1_3107` | Add ledger setting columns if missing |
| 3 | `softaccdb_1_3107` | `CREATE OR REPLACE PROCEDURE` + `CALL sp_seed_company_accounting_data(10004)` |
| 4 | `softaccdb_1_3107` | Fiscal year + Admin user/role |
| 5 | `softaccdb_1_3107` | Admin full `tab_role_permission` |
| 6 | App | Login as Admin / Company 10004 |

Executable copy of all tenant steps (after Step 1):  
`SoftBook_Backend_API/Scripts/Register_Company_10004_Tenant.sql`

---

## Clone existing company data (10001 → 10004)

To copy **all** company-scoped data from live company `10001` into `10004` on the same tenant DB:

1. Ensure `companymaster` already has company `10004` pointing at `softaccdb_1_3107`.
2. **Back up** `softaccdb_1_3107`.
3. Run on tenant DB: [`Scripts/Clone_Company_10001_to_10004.sql`](../Scripts/Clone_Company_10001_to_10004.sql)

Behavior:

- Wipes existing `10004` rows first (skips shared `tabs`)
- Clones masters + SoftBook transactions (`tab_sale_pur`, `tab_sal_pur_tran`, `tab_jv` via `jv_comp_code`, `tab_pay_rec`, `mob_order*`)
- Remaps PKs/FKs via temp `map_generic`
- Does **not** use `Mass_Update_Company_ID.sql` (that renames in place and would destroy 10001)
- After clone, login to `10004` with users/passwords copied from `10001`

---

## Related repo files

| File | Role |
|------|------|
| `SoftBook.API/appsettings.json` | Master DB connection |
| `SoftBook.Infrastructure/Postgres_CompanyMaster.sql` | `tab_company_master` DDL |
| `SoftBook.Infrastructure/Seed_CompanyMaster.sql` | Sample master insert |
| `Scripts/Register_Company_10004_CompanyMaster.sql` | Step 1 routing insert |
| `Scripts/Register_Company_10004_Tenant.sql` | Minimal seed (no groups/voucher types) |
| `Scripts/Clone_Company_10001_to_10004.sql` | Full wipe+clone 10001→10004 |
| `sp_seed_company_accounting_data.sql` | Accounting seed procedure (optional) |
| `Seed_Accounting_10001.sql` | Older seed wrapper (has `PERFORM` bug — use register scripts instead) |
| `SoftBook.Application/Helpers/PasswordHelper.cs` | SHA256 Base64 password hash |
