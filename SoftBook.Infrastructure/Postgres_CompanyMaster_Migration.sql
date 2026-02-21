-----------------------------------------------------------
-- POSTGRESQL COMPANY MASTER SCHEMA (REF: TabCompanyMaster)
-- DATABASE: CompanyMaster (Global Management)
-- CONVENTION: snake_case
-- PURPOSE: Routing and metadata for multi-tenant sharding
-----------------------------------------------------------

CREATE TABLE IF NOT EXISTS tab_company_master (
    code SERIAL PRIMARY KEY,                      -- Internal auto-increment
    company_id INT UNIQUE NOT NULL,               -- Unique logical ID used in tenant tables
    company_name VARCHAR(100) NOT NULL,           -- Legal name
    password VARCHAR(100),                        -- Admin password / Security
    connection_string VARCHAR(500) NOT NULL,      -- Points to the specific PostgreSQL SoftAccDb shard
    is_enabled BOOLEAN DEFAULT TRUE,              -- Toggle access
    priority INT DEFAULT 0,                       -- Priority level for load balancing or sorting
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Index for fast routing by CompanyID
CREATE INDEX IF NOT EXISTS idx_company_routing ON tab_company_master(company_id);

-----------------------------------------------------------
-- Company lookup (replacement for SQL Server sp_GetCompanyById)
-- Usage: SELECT * FROM sp_getcompanybyid(123);
-----------------------------------------------------------
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