//using Microsoft.Extensions.Configuration;
//using Npgsql;
//using SoftBook.Domain.Data;
//using SoftBook.Domain.DTOs;
//using SoftBook.Domain.RepositoryInterfaces;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Threading.Tasks;

//namespace SoftBook.Infrastructure.Classes
//{
//    public class ChatRepository : BaseDAL, IChatRepository
//    {
//        public ChatRepository(
//            IConfiguration configuration,
//            ITenantProvider tenantProvider)
//            : base(configuration, tenantProvider)
//        {
//        }

//        public async Task<Message> SendMessageAsync(Message message)
//        {
//            string query = @"
//        INSERT INTO tab_messages
//        (
//            company_id,
//            sender_user_id,
//            receiver_user_id,
//            message_text,
//            message_type,
//            file_link,
//            read_at,
//            reply_message_id,
//            created_by,
//            created_date
//        )
//        VALUES
//        (
//            @CompanyId,
//            @SenderUserId,
//            @ReceiverUserId,
//            @MessageText,
//            @MessageType,
//            @FileLink,
//            NULL,
//            @ReplyMessageId,
//            @CreatedBy,
//            NOW()
//        )
//        RETURNING *;
//    ";

//            var result = await ExecuteRawQueryAsync(query, cmd =>
//            {
//                cmd.Parameters.AddWithValue("@CompanyId", message.CompanyId);

//                cmd.Parameters.AddWithValue("@SenderUserId", message.SenderUserId);

//                cmd.Parameters.AddWithValue("@ReceiverUserId", message.ReceiverUserId);

//                cmd.Parameters.AddWithValue("@MessageText",
//                    (object?)message.MessageText ?? DBNull.Value);

//                cmd.Parameters.AddWithValue("@MessageType",
//                    message.MessageType ?? "text");

//                cmd.Parameters.AddWithValue("@FileLink",
//                    (object?)message.FileLink ?? DBNull.Value);

//                cmd.Parameters.AddWithValue("@ReplyMessageId",
//                    (object?)message.ReplyMessageId ?? DBNull.Value);

//                cmd.Parameters.AddWithValue("@CreatedBy",
//                    (object?)message.CreatedBy ?? DBNull.Value);

//            }, MapMessageReader);

//            return result.FirstOrDefault();
//        }
//        public async Task<List<Message>> GetConversationAsync(
//            long senderId,
//            long receiverId)
//        {
//            string query = @"
//                SELECT *
//                FROM tab_messages
//                WHERE
//                (
//                    (
//                        sender_user_id = @SenderId
//                        AND receiver_user_id = @ReceiverId
//                    )
//                    OR
//                    (
//                        sender_user_id = @ReceiverId
//                        AND receiver_user_id = @SenderId
//                    )
//                )
//                AND is_deleted = false
//                ORDER BY created_date ASC;
//            ";

//            return await ExecuteRawQueryAsync(query, cmd =>
//            {
//                cmd.Parameters.AddWithValue("@SenderId", senderId);

//                cmd.Parameters.AddWithValue("@ReceiverId", receiverId);

//            }, MapMessageReader);
//        }

//        private static Message MapMessageReader(NpgsqlDataReader reader) => new Message
//        {
//            MessageId = Convert.ToInt64(reader["message_id"]),

//            CompanyId = Convert.ToInt64(reader["company_id"]),

//            SenderUserId = Convert.ToInt64(reader["sender_user_id"]),

//            ReceiverUserId = Convert.ToInt64(reader["receiver_user_id"]),

//            MessageText = reader["message_text"]?.ToString(),

//            MessageType = reader["message_type"]?.ToString() ?? "text",

//            FileLink = reader["file_link"]?.ToString(),

//            ReadAt = reader["read_at"] != DBNull.Value
//                ? Convert.ToDateTime(reader["read_at"])
//                : (DateTime?)null,

//            IsDeleted = Convert.ToBoolean(reader["is_deleted"]),

//            IsEdited = Convert.ToBoolean(reader["is_edited"]),

//            ReplyMessageId = reader["reply_message_id"] != DBNull.Value
//                ? Convert.ToInt64(reader["reply_message_id"])
//                : (long?)null,

//            CreatedBy = reader["created_by"] != DBNull.Value
//                ? Convert.ToInt64(reader["created_by"])
//                : (long?)null,

//            CreatedDate = Convert.ToDateTime(reader["created_date"]),

//            UpdatedBy = reader["updated_by"] != DBNull.Value
//                ? Convert.ToInt64(reader["updated_by"])
//                : (long?)null,

//            UpdatedDate = reader["updated_date"] != DBNull.Value
//                ? Convert.ToDateTime(reader["updated_date"])
//                : (DateTime?)null
//        };

//        public async Task MarkMessagesAsReadAsync(
//    long senderId,
//    long receiverId)
//        {
//            string query = @"
//        UPDATE tab_messages
//        SET read_at = NOW()
//        WHERE
//            sender_user_id = @SenderId
//            AND receiver_user_id = @ReceiverId
//            AND read_at IS NULL
//            AND is_deleted = false;
//    ";

//            await ExecuteNonQueryAsync(query, cmd =>
//            {
//                cmd.Parameters.AddWithValue("@SenderId", senderId);

//                cmd.Parameters.AddWithValue("@ReceiverId", receiverId);

//            }, CommandType.Text);
//        }

//        public async Task<Message?> GetMessageByIdAsync(long messageId)
//        {
//            string query = @"
//                SELECT *
//                FROM tab_messages
//                WHERE message_id = @MessageId
//                    AND is_deleted = false
//                LIMIT 1;
//            ";

//            var result = await ExecuteRawQueryAsync(query, cmd =>
//            {
//                cmd.Parameters.AddWithValue("@MessageId", messageId);
//            }, MapMessageReader);

//            return result.FirstOrDefault();
//        }

//        public async Task<bool> DeleteMessageAsync(long messageId, long senderUserId)
//        {
//            string query = @"
//                WITH updated AS
//                (
//                    UPDATE tab_messages
//                    SET
//                        is_deleted = true,
//                        updated_date = NOW()
//                    WHERE message_id = @MessageId
//                        AND sender_user_id = @SenderUserId
//                        AND is_deleted = false
//                    RETURNING 1
//                )
//                SELECT COUNT(*) FROM updated;
//            ";

//            var affectedRows = await ExecuteScalarAsync<int>(query, cmd =>
//            {
//                cmd.Parameters.AddWithValue("@MessageId", messageId);
//                cmd.Parameters.AddWithValue("@SenderUserId", senderUserId);

//            }, CommandType.Text);

//            return affectedRows > 0;
//        }

//        public async Task<List<ChatListModel>> GetChatListAsync(
//            long userId,
//            long companyId)
//        {
//            string query = @"
//        SELECT
//            u.user_id AS user_id,

//            u.username AS user_name,

//            m.message_text AS last_message,

//            m.created_date AS last_message_date,

//            (
//                SELECT COUNT(*)
//                FROM tab_messages um
//                WHERE
//                    um.sender_user_id = u.user_id
//                    AND um.receiver_user_id = @UserId
//                    AND um.read_at IS NULL
//                    AND um.is_deleted = false
//                    AND um.company_id = @CompanyId
//            ) AS unread_count

//        FROM tab_user_master u

//        INNER JOIN
//        (
//            SELECT DISTINCT ON
//            (
//                CASE
//                    WHEN sender_user_id = @UserId
//                    THEN receiver_user_id
//                    ELSE sender_user_id
//                END
//            )

//            message_id,
//            sender_user_id,
//            receiver_user_id,
//            message_text,
//            created_date,
//            company_id

//            FROM tab_messages

//            WHERE
//            (
//                sender_user_id = @UserId
//                OR receiver_user_id = @UserId
//            )
//            AND company_id = @CompanyId
//            AND is_deleted = false

//            ORDER BY
//            CASE
//                WHEN sender_user_id = @UserId
//                THEN receiver_user_id
//                ELSE sender_user_id
//            END,
//            created_date DESC

//        ) m

//        ON u.user_id =
//        CASE
//            WHEN m.sender_user_id = @UserId
//            THEN m.receiver_user_id
//            ELSE m.sender_user_id
//        END

//        WHERE u.company_id = @CompanyId

//        ORDER BY m.created_date DESC;
//    ";

//            return await ExecuteRawQueryAsync(query, cmd =>
//            {
//                cmd.Parameters.AddWithValue("@UserId", userId);

//                cmd.Parameters.AddWithValue("@CompanyId", companyId);

//            }, MapChatListReader);
//        }

//        private static ChatListModel MapChatListReader(NpgsqlDataReader reader)
//            => new ChatListModel
//            {
//                UserId = Convert.ToInt64(reader["user_id"]),

//                UserName = reader["user_name"]?.ToString() ?? string.Empty,

//                LastMessage = reader["last_message"]?.ToString(),

//                LastMessageDate = reader["last_message_date"] != DBNull.Value
//                ? Convert.ToDateTime(reader["last_message_date"])
//                : (DateTime?)null,

//                UnreadCount = Convert.ToInt32(reader["unread_count"])
//            };


//        public async Task<List<ChatAttachmentDto>>  GetChatAttachmentsAsync(
//      long senderUserId,
//      long receiverUserId,
//      long companyId)

//        {
//            string query = @"

//    SELECT
//        message_id,
//        message_text,
//        file_link,
//        message_type,
//        created_date

//    FROM tab_messages

//    WHERE
//        message_type = 'file'
//        AND is_deleted = false
//        AND company_id = @CompanyId

//        AND
//        (
//            (
//                sender_user_id = @SenderUserId
//                AND receiver_user_id = @ReceiverUserId
//            )

//            OR

//            (
//                sender_user_id = @ReceiverUserId
//                AND receiver_user_id = @SenderUserId
//            )
//        )

//    ORDER BY created_date DESC;

//    ";

//            return await ExecuteRawQueryAsync(query, cmd =>
//            {
//                cmd.Parameters.AddWithValue(
//                    "@SenderUserId",
//                    senderUserId);

//                cmd.Parameters.AddWithValue(
//                    "@ReceiverUserId",
//                    receiverUserId);

//                cmd.Parameters.AddWithValue(
//                    "@CompanyId",
//                    companyId);

//            }, MapChatAttachmentReader);
//        }

//        private ChatAttachmentDto MapChatAttachmentReader(
//    NpgsqlDataReader reader)
//        {
//            return new ChatAttachmentDto
//            {
//                MessageId =
//                    reader["message_id"] != DBNull.Value
//                    ? Convert.ToInt64(reader["message_id"])
//                    : 0,

//                FileName =
//                    reader["message_text"]?.ToString(),

//                FileUrl =
//                    reader["file_link"]?.ToString(),

//                MessageType =
//                    reader["message_type"]?.ToString(),

//                CreatedDate =
//                    reader["created_date"] != DBNull.Value
//                    ? Convert.ToDateTime(reader["created_date"])
//                    : DateTime.MinValue
//            };
//        }


//    }
//}
using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SoftBook.Infrastructure.Classes
{
    public class ChatRepository : BaseDAL, IChatRepository
    {
        public ChatRepository(
            IConfiguration configuration,
            ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            // Direct INSERT: reliable with tenant DBs; fn_chat_actions SEND_MESSAGE needs
            // #variable_conflict use_column + INSERT alias on the tenant database.
            string query = @"
                INSERT INTO tab_messages
                (
                    company_id, sender_user_id, receiver_user_id,
                    message_text, message_type, file_link,
                    reply_message_id, created_by, created_date
                )
                VALUES
                (
                    @CompanyId, @SenderUserId, @ReceiverUserId,
                    @MessageText, COALESCE(@MessageType, 'text'), @FileLink,
                    @ReplyMessageId, @CreatedBy, NOW()
                )
                RETURNING *;
            ";

            var result = await ExecuteRawQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@CompanyId", message.CompanyId);
                cmd.Parameters.AddWithValue("@SenderUserId", message.SenderUserId);
                cmd.Parameters.AddWithValue("@ReceiverUserId", message.ReceiverUserId);
                cmd.Parameters.AddWithValue("@MessageText",
                    (object?)message.MessageText ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MessageType",
                    (object?)message.MessageType ?? "text");
                cmd.Parameters.AddWithValue("@FileLink",
                    (object?)message.FileLink ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ReplyMessageId",
                    (object?)message.ReplyMessageId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy",
                    (object?)message.CreatedBy ?? DBNull.Value);

            }, MapMessageReader);

            var saved = result.FirstOrDefault();
            if (saved == null)
            {
                throw new InvalidOperationException("Message was not saved to the database.");
            }

            return saved;
        }

        public async Task<List<Message>> GetConversationAsync(
            long senderId,
            long receiverId)
        {
            string query = @"
                SELECT * FROM fn_chat_actions(
                    p_action_type       => 'GET_CONVERSATION',
                    p_sender_user_id    => @SenderId,
                    p_receiver_user_id  => @ReceiverId
                );
            ";

            return await ExecuteRawQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@SenderId", senderId);
                cmd.Parameters.AddWithValue("@ReceiverId", receiverId);

            }, MapMessageReader);
        }

        public async Task MarkMessagesAsReadAsync(
            long senderId,
            long receiverId)
        {
            string query = @"
                SELECT fn_chat_actions(
                    p_action_type       => 'MARK_AS_READ',
                    p_sender_user_id    => @SenderId,
                    p_receiver_user_id  => @ReceiverId
                );
            ";

            await ExecuteNonQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@SenderId", senderId);
                cmd.Parameters.AddWithValue("@ReceiverId", receiverId);

            }, CommandType.Text);
        }

        public async Task<Message?> GetMessageByIdAsync(long messageId)
        {
            // Not yet in fn_chat_actions — kept as raw SQL
            string query = @"
                SELECT *
                FROM tab_messages
                WHERE message_id = @MessageId
                    AND is_deleted = false
                LIMIT 1;
            ";

            var result = await ExecuteRawQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@MessageId", messageId);
            }, MapMessageReader);

            return result.FirstOrDefault();
        }

        public async Task<bool> DeleteMessageAsync(long messageId, long senderUserId)
        {
            // Direct UPDATE: reliable on tenant DBs (same approach as SendMessageAsync).
            string query = @"
                UPDATE tab_messages
                SET is_deleted = true, updated_date = NOW()
                WHERE message_id = @MessageId
                    AND sender_user_id = @SenderUserId
                    AND is_deleted = false
                RETURNING *;
            ";

            var result = await ExecuteRawQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@MessageId", messageId);
                cmd.Parameters.AddWithValue("@SenderUserId", senderUserId);

            }, MapMessageReader);

            return result.Count > 0;
        }

        public async Task<List<ChatListModel>> GetChatListAsync(
            long userId,
            long companyId)
        {
            // Not yet in fn_chat_actions — kept as raw SQL
            string query = @"
                SELECT
                    u.user_id           AS user_id,
                    u.username          AS user_name,
                    m.message_text      AS last_message,
                    m.created_date      AS last_message_date,
                    (
                        SELECT COUNT(*)
                        FROM tab_messages um
                        WHERE
                            um.sender_user_id    = u.user_id
                            AND um.receiver_user_id  = @UserId
                            AND um.read_at        IS NULL
                            AND um.is_deleted     = false
                            AND um.company_id     = @CompanyId
                    ) AS unread_count
                FROM tab_user_master u
                INNER JOIN
                (
                    SELECT DISTINCT ON
                    (
                        CASE
                            WHEN sender_user_id = @UserId THEN receiver_user_id
                            ELSE sender_user_id
                        END
                    )
                    message_id, sender_user_id, receiver_user_id,
                    message_text, created_date, company_id
                    FROM tab_messages
                    WHERE
                        (sender_user_id = @UserId OR receiver_user_id = @UserId)
                        AND company_id  = @CompanyId
                        AND is_deleted  = false
                    ORDER BY
                        CASE
                            WHEN sender_user_id = @UserId THEN receiver_user_id
                            ELSE sender_user_id
                        END,
                        created_date DESC
                ) m
                ON u.user_id =
                    CASE
                        WHEN m.sender_user_id = @UserId THEN m.receiver_user_id
                        ELSE m.sender_user_id
                    END
                WHERE u.company_id = @CompanyId
                ORDER BY m.created_date DESC;
            ";

            return await ExecuteRawQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CompanyId", companyId);

            }, MapChatListReader);
        }

        public async Task<List<ChatAttachmentDto>> GetChatAttachmentsAsync(
            long senderUserId,
            long receiverUserId,
            long companyId)
        {
            // Not yet in fn_chat_actions — kept as raw SQL
            string query = @"
                SELECT
                    message_id, message_text, file_link,
                    message_type, created_date
                FROM tab_messages
                WHERE
                    message_type = 'file'
                    AND is_deleted   = false
                    AND company_id   = @CompanyId
                    AND
                    (
                        (sender_user_id = @SenderUserId   AND receiver_user_id = @ReceiverUserId)
                        OR
                        (sender_user_id = @ReceiverUserId AND receiver_user_id = @SenderUserId)
                    )
                ORDER BY created_date DESC;
            ";

            return await ExecuteRawQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@SenderUserId", senderUserId);
                cmd.Parameters.AddWithValue("@ReceiverUserId", receiverUserId);
                cmd.Parameters.AddWithValue("@CompanyId", companyId);

            }, MapChatAttachmentReader);
        }

        // ── Mappers ────────────────────────────────────────────────────────────

        private static bool ReadBool(NpgsqlDataReader reader, string column, bool defaultValue = false)
        {
            var value = reader[column];
            return value == DBNull.Value ? defaultValue : Convert.ToBoolean(value);
        }

        private static Message MapMessageReader(NpgsqlDataReader reader) => new Message
        {
            MessageId = Convert.ToInt64(reader["message_id"]),
            CompanyId = Convert.ToInt64(reader["company_id"]),
            SenderUserId = Convert.ToInt64(reader["sender_user_id"]),
            ReceiverUserId = Convert.ToInt64(reader["receiver_user_id"]),
            MessageText = reader["message_text"]?.ToString(),
            MessageType = reader["message_type"]?.ToString() ?? "text",
            FileLink = reader["file_link"]?.ToString(),
            ReadAt = reader["read_at"] != DBNull.Value
                                ? Convert.ToDateTime(reader["read_at"])
                                : (DateTime?)null,
            IsDeleted = ReadBool(reader, "is_deleted"),
            IsEdited = ReadBool(reader, "is_edited"),
            ReplyMessageId = reader["reply_message_id"] != DBNull.Value
                                ? Convert.ToInt64(reader["reply_message_id"])
                                : (long?)null,
            CreatedBy = reader["created_by"] != DBNull.Value
                                ? Convert.ToInt64(reader["created_by"])
                                : (long?)null,
            CreatedDate = Convert.ToDateTime(reader["created_date"]),
            UpdatedBy = reader["updated_by"] != DBNull.Value
                                ? Convert.ToInt64(reader["updated_by"])
                                : (long?)null,
            UpdatedDate = reader["updated_date"] != DBNull.Value
                                ? Convert.ToDateTime(reader["updated_date"])
                                : (DateTime?)null
        };

        private static ChatListModel MapChatListReader(NpgsqlDataReader reader)
            => new ChatListModel
            {
                UserId = Convert.ToInt64(reader["user_id"]),
                UserName = reader["user_name"]?.ToString() ?? string.Empty,
                LastMessage = reader["last_message"]?.ToString(),
                LastMessageDate = reader["last_message_date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["last_message_date"])
                                    : (DateTime?)null,
                UnreadCount = Convert.ToInt32(reader["unread_count"])
            };

        private ChatAttachmentDto MapChatAttachmentReader(NpgsqlDataReader reader)
            => new ChatAttachmentDto
            {
                MessageId = reader["message_id"] != DBNull.Value
                                ? Convert.ToInt64(reader["message_id"]) : 0,
                FileName = reader["message_text"]?.ToString(),
                FileUrl = reader["file_link"]?.ToString(),
                MessageType = reader["message_type"]?.ToString(),
                CreatedDate = reader["created_date"] != DBNull.Value
                                ? Convert.ToDateTime(reader["created_date"])
                                : DateTime.MinValue
            };

        public async Task<long?> GetCompanyIdForConversationAsync(long userId1, long userId2)
        {
            string query = @"
                SELECT company_id
                FROM tab_messages
                WHERE is_deleted = false
                AND (
                    (sender_user_id = @UserId1 AND receiver_user_id = @UserId2)
                    OR (sender_user_id = @UserId2 AND receiver_user_id = @UserId1)
                )
                ORDER BY created_date DESC
                LIMIT 1";

            return await ExecuteScalarAsync<long?>(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@UserId1", userId1);
                    cmd.Parameters.AddWithValue("@UserId2", userId2);
                },
                CommandType.Text);
        }
    }
}