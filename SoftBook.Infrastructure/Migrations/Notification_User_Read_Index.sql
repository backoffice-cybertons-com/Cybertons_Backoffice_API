-- Improves unread notification lookups for bell badge and listing
CREATE INDEX IF NOT EXISTS idx_notification_user
ON tab_notifications (user_id, is_read);
