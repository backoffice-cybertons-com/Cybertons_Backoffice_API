-- Report Party Type master (sub-classification under Party Type)
-- Example rows:
--   party_type = 'Expense', report_party_type = 'Transport'
--   party_type = 'Asset',   report_party_type = 'Land'
--
-- Used by Generic Master UI: /masters/reportPartyTypes
-- Wired via sp_master_universal master name: reportPartyTypes

CREATE TABLE IF NOT EXISTS public.tab_party_report_type_master
(
    id BIGSERIAL PRIMARY KEY,
    company_id INTEGER NOT NULL,
    party_type character varying(255) NOT NULL,
    report_party_type VARCHAR(150) NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS uq_party_report_type_company_type_name
    ON public.tab_party_report_type_master (company_id, lower(party_type), lower(report_party_type));
