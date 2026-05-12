-- SQL script to create the task templates table
CREATE TABLE IF NOT EXISTS tab_task_templates (
    template_id BIGSERIAL PRIMARY KEY,
    template_name VARCHAR(255) NOT NULL,
    description TEXT,
    default_priority SMALLINT NOT NULL DEFAULT 2, -- 1: Low, 2: Medium, 3: High, 4: Urgent
    default_status SMALLINT NOT NULL DEFAULT 1,   -- 1: Pending, 2: In Progress, 3: Completed, 4: Cancelled
    default_assigned_user_id BIGINT,
    template_color VARCHAR(50) DEFAULT 'bg-info-subtle',
    created_date TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_date TIMESTAMP,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- Optional: Seed some initial templates
-- INSERT INTO tab_task_templates (template_name, description, default_priority, default_status, template_color)
-- VALUES 
-- ('Sales Followup', 'Follow up with customer regarding sales query', 2, 1, 'bg-primary-subtle'),
-- ('Customer Meeting', 'Standard meeting with client', 3, 1, 'bg-success-subtle'),
-- ('GST Filing', 'Monthly GST filing reminder', 4, 1, 'bg-danger-subtle');
