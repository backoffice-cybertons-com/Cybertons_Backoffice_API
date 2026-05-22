-- Chat actions function (used by ChatRepository.cs)
-- Fix: INSERT RETURNING must use column names only (no table alias "m")

CREATE OR REPLACE FUNCTION fn_chat_actions
(
    p_action_type TEXT,
    p_company_id BIGINT DEFAULT NULL,
    p_sender_user_id BIGINT DEFAULT NULL,
    p_receiver_user_id BIGINT DEFAULT NULL,
    p_message_id BIGINT DEFAULT NULL,
    p_message_text TEXT DEFAULT NULL,
    p_message_type TEXT DEFAULT NULL,
    p_file_link TEXT DEFAULT NULL,
    p_reply_message_id BIGINT DEFAULT NULL,
    p_created_by BIGINT DEFAULT NULL
)
RETURNS TABLE
(
    message_id BIGINT,
    company_id BIGINT,
    sender_user_id BIGINT,
    receiver_user_id BIGINT,
    message_text TEXT,
    message_type TEXT,
    file_link TEXT,
    read_at TIMESTAMP,
    is_deleted BOOLEAN,
    is_edited BOOLEAN,
    reply_message_id BIGINT,
    created_by BIGINT,
    created_date TIMESTAMP,
    updated_by BIGINT,
    updated_date TIMESTAMP
)
LANGUAGE plpgsql
AS
$$
#variable_conflict use_column
BEGIN
    IF p_action_type = 'SEND_MESSAGE' THEN
        RETURN QUERY
        INSERT INTO tab_messages AS ins
        (
            company_id, sender_user_id, receiver_user_id,
            message_text, message_type, file_link,
            reply_message_id, created_by, created_date
        )
        VALUES
        (
            p_company_id, p_sender_user_id, p_receiver_user_id,
            p_message_text, COALESCE(p_message_type, 'text'), p_file_link,
            p_reply_message_id, p_created_by, NOW()
        )
        RETURNING
            ins.message_id,
            ins.company_id,
            ins.sender_user_id,
            ins.receiver_user_id,
            ins.message_text::TEXT,
            ins.message_type::TEXT,
            ins.file_link::TEXT,
            ins.read_at,
            ins.is_deleted,
            ins.is_edited,
            ins.reply_message_id,
            ins.created_by,
            ins.created_date,
            ins.updated_by,
            ins.updated_date;

    ELSIF p_action_type = 'GET_CONVERSATION' THEN
        RETURN QUERY
        SELECT
            m.message_id, m.company_id, m.sender_user_id, m.receiver_user_id,
            m.message_text::TEXT, m.message_type::TEXT, m.file_link::TEXT,
            m.read_at, m.is_deleted, m.is_edited, m.reply_message_id,
            m.created_by, m.created_date, m.updated_by, m.updated_date
        FROM tab_messages m
        WHERE
        (
            (m.sender_user_id = p_sender_user_id AND m.receiver_user_id = p_receiver_user_id)
            OR
            (m.sender_user_id = p_receiver_user_id AND m.receiver_user_id = p_sender_user_id)
        )
        AND m.is_deleted = false
        ORDER BY m.created_date ASC;

    ELSIF p_action_type = 'DELETE_MESSAGE' THEN
        RETURN QUERY
        UPDATE tab_messages m
        SET is_deleted = true, updated_date = NOW()
        WHERE
            m.message_id = p_message_id
            AND m.sender_user_id = p_sender_user_id
            AND m.is_deleted = false
        RETURNING
            m.message_id, m.company_id, m.sender_user_id, m.receiver_user_id,
            m.message_text::TEXT, m.message_type::TEXT, m.file_link::TEXT,
            m.read_at, m.is_deleted, m.is_edited, m.reply_message_id,
            m.created_by, m.created_date, m.updated_by, m.updated_date;

    ELSIF p_action_type = 'MARK_AS_READ' THEN
        UPDATE tab_messages
        SET read_at = NOW()
        WHERE
            sender_user_id = p_sender_user_id
            AND receiver_user_id = p_receiver_user_id
            AND read_at IS NULL
            AND is_deleted = false;
        RETURN;
    END IF;
END;
$$;
