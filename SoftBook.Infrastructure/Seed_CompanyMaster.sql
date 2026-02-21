-- SEED DATA FOR COMPANY MASTER
-- Run this in 'companymaster' database

INSERT INTO tab_company_master (
    company_id, 
    company_name, 
    code,
    connection_string, 
    is_enabled, 
    priority
) 
VALUES (
    1, 
    'SoftBook Demo Company', 
    1,
    'Host=localhost;Database=softaccdb_1;Username=postgres;Password=Soft123', 
    TRUE, 
    1
)
ON CONFLICT (company_id) DO NOTHING;
