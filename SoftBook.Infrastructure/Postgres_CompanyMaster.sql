-----------------------------------------------------------
-- SOFTBOOK POSTGRESQL COMPLETE DATABASE SCRIPT
-- GENERATED ON: 2026-01-29
-- CONVENTION: snake_case
-- DATABASE: SoftAccDb (Shared Database)
-----------------------------------------------------------

-- 0. CLEANUP (Optional - Uncomment to reset)
-- DROP SCHEMA IF EXISTS public CASCADE;
-- CREATE SCHEMA public;

-----------------------------------------------------------
-- 1. COMPANY MASTER (Global Management)
-----------------------------------------------------------

CREATE TABLE IF NOT EXISTS tab_company_master (
    code SERIAL PRIMARY KEY,
    company_id INT UNIQUE NOT NULL,
    company_name VARCHAR(100) NOT NULL,
    password VARCHAR(100),
    connection_string VARCHAR(500) NOT NULL,
    is_enabled BOOLEAN DEFAULT TRUE,
    priority INT DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_company_routing ON tab_company_master(company_id);

CREATE OR REPLACE FUNCTION sp_getcompanybyid(p_company_id INT)
RETURNS TABLE (
    company_id INT,
    company_name VARCHAR(100),
    connection_string VARCHAR(500),
    is_enabled BOOLEAN,
    priority INT,
    created_at TIMESTAMPTZ
)
LANGUAGE sql
AS $$
    SELECT
        t.company_id,
        t.company_name,
        t.connection_string,
        t.is_enabled,
        t.priority,
        t.created_at
    FROM tab_company_master t
    WHERE t.company_id = p_company_id;
$$;

-----------------------------------------------------------
