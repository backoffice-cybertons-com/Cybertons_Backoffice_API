-- Extends existing tab_system_log for dashboard "System Log Datewise" card.
-- Your base table: log_id, log_time, module_name, message, user_id

-- Required for per-company filtering and Login/Logout/Add/Edit/Delete counts
ALTER TABLE public.tab_system_log
    ADD COLUMN IF NOT EXISTS action_type VARCHAR(20) NULL;

ALTER TABLE public.tab_system_log
    ADD COLUMN IF NOT EXISTS company_id INT NULL;

COMMENT ON COLUMN public.tab_system_log.action_type IS 'Login | Logout | Add | Edit | Delete';
COMMENT ON COLUMN public.tab_system_log.module_name IS 'Master/screen name e.g. party, product, sales';
COMMENT ON COLUMN public.tab_system_log.message IS 'Optional detail text';

CREATE INDEX IF NOT EXISTS idx_tab_system_log_company_time
    ON public.tab_system_log (company_id, (log_time::date));

CREATE INDEX IF NOT EXISTS idx_tab_system_log_user_action
    ON public.tab_system_log (company_id, user_id, action_type);
