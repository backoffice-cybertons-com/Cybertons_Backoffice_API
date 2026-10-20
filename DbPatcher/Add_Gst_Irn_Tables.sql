-- GST E-Invoice (IRN) tables for SoftBook
-- Run against company PostgreSQL database

CREATE TABLE IF NOT EXISTS public.tab_gst_irn_settings (
    company_id      INTEGER PRIMARY KEY,
    mode            VARCHAR(20) NOT NULL DEFAULT 'Mock',
    auth_url        TEXT,
    e_invoice_url   TEXT,
    client_id       VARCHAR(200),
    client_secret   VARCHAR(500),
    api_username    VARCHAR(100),
    api_password    VARCHAR(200),
    public_key      TEXT,
    gstin           VARCHAR(15),
    updated_on      TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS public.tab_gst_irn (
    irn_id          SERIAL PRIMARY KEY,
    sp_unique_code  INTEGER NOT NULL,
    company_id      INTEGER NOT NULL,
    inv_no          VARCHAR(50),
    ack_no          BIGINT,
    ack_dt          TIMESTAMPTZ,
    irn             VARCHAR(128),
    signed_invoice  TEXT,
    signed_qr_code  TEXT,
    qr_image        BYTEA,
    status          VARCHAR(20) NOT NULL DEFAULT 'SUCCESS',
    is_mock         BOOLEAN NOT NULL DEFAULT FALSE,
    error_code      TEXT,
    error_message   TEXT,
    created_by      INTEGER,
    created_on      TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_tab_gst_irn_sp_code
    ON public.tab_gst_irn (company_id, sp_unique_code);
