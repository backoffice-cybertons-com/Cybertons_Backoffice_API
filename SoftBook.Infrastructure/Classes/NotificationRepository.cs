using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Constants;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SoftBook.Domain.Data;

namespace SoftBook.Infrastructure.Classes
{
    public class NotificationRepository : BaseDAL, INotificationRepository
    {
        public NotificationRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        private static NotificationModel MapReader(IDataReader reader) => new NotificationModel
        {
            NotificationId = Convert.ToInt64(reader["notification_id"]),
            CompanyId = Convert.ToInt64(reader["company_id"]),
            UserId = Convert.ToInt64(reader["user_id"]),
            Title = reader["title"]?.ToString(),
            Message = reader["message"]?.ToString(),
            ReferenceType = reader["reference_type"]?.ToString(),
            ReferenceId = reader["reference_id"] == DBNull.Value ? null : (long?)Convert.ToInt64(reader["reference_id"]),
            IsRead = Convert.ToBoolean(reader["is_read"]),
            CreatedDate = Convert.ToDateTime(reader["created_date"]),
            IsDeleted = Convert.ToBoolean(reader["is_deleted"]),
            ToastShown = Convert.ToBoolean(reader["toast_shown"]),
            NotificationCount = reader["notification_count"] == DBNull.Value
                ? 1
                : Convert.ToInt32(reader["notification_count"]),
            SenderUserId = reader["sender_user_id"] == DBNull.Value
                ? null
                : (long?)Convert.ToInt64(reader["sender_user_id"])
        };

        public async Task<List<NotificationModel>> GetUnreadNotifications(long companyId, long userId)
        {
            string query = @"
                SELECT * FROM tab_notifications 
                WHERE company_id = @CompanyId 
                AND user_id = @UserId 
                AND is_read = false 
                AND created_date >= NOW() - INTERVAL '3 days'
                AND is_deleted = false
                ORDER BY created_date DESC";

            return await ExecuteRawQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                },
                MapReader);
        }

        public async Task<bool> MarkAsRead(long companyId, long notificationId)
        {
            string query = @"
                UPDATE tab_notifications 
                SET is_read = true 
                WHERE notification_id = @NotificationId
                AND company_id = @CompanyId";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@NotificationId", notificationId);
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                },
                CommandType.Text);

            return true;
        }

        public async Task<List<NotificationModel>> GetAllNotifications(long companyId, long userId, int page = 1, int pageSize = 20)
        {
            int offset = (page - 1) * pageSize;
            string query = @"
                SELECT * FROM tab_notifications 
                WHERE company_id = @CompanyId 
                AND user_id = @UserId 
                AND is_deleted = false
                ORDER BY created_date DESC
                LIMIT @PageSize OFFSET @Offset";

            return await ExecuteRawQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@Offset", offset);
                },
                MapReader);
        }

        public async Task<long> AddNotification(NotificationModel notification)
        {
            string query = @"
                INSERT INTO tab_notifications 
                (
                    company_id, 
                    user_id, 
                    title, 
                    message, 
                    reference_type, 
                    reference_id, 
                    created_date, 
                    is_read, 
                    is_deleted,
                    toast_shown,
                    notification_count,
                    sender_user_id
                ) 
                VALUES 
                (
                    @CompanyId, 
                    @UserId, 
                    @Title, 
                    @Message, 
                    @ReferenceType, 
                    @ReferenceId, 
                    NOW(), 
                    false, 
                    false,
                    false,
                    @NotificationCount,
                    @SenderUserId
                ) 
                RETURNING notification_id";

            return await ExecuteScalarAsync<long>(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", notification.CompanyId);
                    cmd.Parameters.AddWithValue("@UserId", notification.UserId);
                    cmd.Parameters.AddWithValue("@Title", notification.Title ?? "");
                    cmd.Parameters.AddWithValue("@Message", notification.Message ?? "");
                    cmd.Parameters.AddWithValue("@ReferenceType", notification.ReferenceType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReferenceId", notification.ReferenceId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NotificationCount", notification.NotificationCount > 0 ? notification.NotificationCount : 1);
                    cmd.Parameters.AddWithValue("@SenderUserId", notification.SenderUserId ?? (object)DBNull.Value);
                },
                CommandType.Text);
        }

        public async Task<bool> MarkToastAsShown(long companyId, long notificationId)
        {
            string query = @"
                UPDATE tab_notifications 
                SET toast_shown = true 
                WHERE notification_id = @NotificationId
                AND company_id = @CompanyId";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@NotificationId", notificationId);
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                },
                CommandType.Text);

            return true;
        }

        public async Task<NotificationModel?> GetUnreadChatNotificationAsync(
            long companyId,
            long receiverUserId,
            long senderUserId)
        {
            string query = @"
                SELECT * FROM tab_notifications
                WHERE company_id = @CompanyId
                AND user_id = @ReceiverUserId
                AND sender_user_id = @SenderUserId
                AND reference_type = 'CHAT'
                AND is_read = false
                AND is_deleted = false
                LIMIT 1";

            var results = await ExecuteRawQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@ReceiverUserId", receiverUserId);
                    cmd.Parameters.AddWithValue("@SenderUserId", senderUserId);
                },
                MapReader);

            return results.FirstOrDefault();
        }

        public async Task UpdateChatNotificationAsync(
            long notificationId,
            string message,
            long referenceId,
            int notificationCount)
        {
            string query = @"
                UPDATE tab_notifications
                SET
                    notification_count = @NotificationCount,
                    message = @Message,
                    reference_id = @ReferenceId,
                    created_date = NOW(),
                    toast_shown = false
                WHERE notification_id = @NotificationId";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@NotificationId", notificationId);
                    cmd.Parameters.AddWithValue("@Message", message ?? "");
                    cmd.Parameters.AddWithValue("@ReferenceId", referenceId);
                    cmd.Parameters.AddWithValue("@NotificationCount", notificationCount);
                },
                CommandType.Text);
        }

        public async Task<string?> GetUserDisplayNameAsync(long userId)
        {
            string query = @"
                SELECT COALESCE(NULLIF(TRIM(full_name), ''), username) AS display_name
                FROM tab_user_master
                WHERE user_id = @UserId
                LIMIT 1";

            return await ExecuteScalarAsync<string?>(
                query,
                cmd => cmd.Parameters.AddWithValue("@UserId", userId),
                CommandType.Text);
        }

        public async Task MarkChatNotificationsAsReadAsync(
            long companyId,
            long receiverUserId,
            long senderUserId)
        {
            string query = @"
                UPDATE tab_notifications
                SET is_read = true
                WHERE company_id = @CompanyId
                AND user_id = @ReceiverUserId
                AND sender_user_id = @SenderUserId
                AND reference_type = 'CHAT'
                AND is_read = false
                AND is_deleted = false";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@ReceiverUserId", receiverUserId);
                    cmd.Parameters.AddWithValue("@SenderUserId", senderUserId);
                },
                CommandType.Text);
        }

        public async Task<List<long>> BulkAddAnnouncementNotificationsAsync(
            long announcementId,
            string title,
            string message)
        {
            const string query = @"
                INSERT INTO tab_notifications
                (
                    company_id,
                    user_id,
                    title,
                    message,
                    reference_type,
                    reference_id,
                    created_date,
                    is_read,
                    is_deleted,
                    toast_shown,
                    notification_count,
                    sender_user_id
                )
                SELECT
                    u.company_id,
                    u.user_id,
                    @Title,
                    @Message,
                    @ReferenceType,
                    @AnnouncementId,
                    NOW(),
                    false,
                    false,
                    false,
                    1,
                    NULL
                FROM tab_user_master u
                WHERE COALESCE(u.is_active, true) = true
                  AND u.company_id > 0
                RETURNING user_id";

            return await ExecuteRawQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Title", title ?? "");
                    cmd.Parameters.AddWithValue("@Message", message ?? "");
                    cmd.Parameters.AddWithValue("@ReferenceType", NotificationReferenceTypes.Announcement);
                    cmd.Parameters.AddWithValue("@AnnouncementId", announcementId);
                },
                reader => Convert.ToInt64(reader["user_id"]));
        }

        public async Task SoftDeleteByReferenceAsync(string referenceType, long referenceId)
        {
            const string query = @"
                UPDATE tab_notifications
                SET is_deleted = true
                WHERE reference_type = @ReferenceType
                  AND reference_id = @ReferenceId
                  AND is_deleted = false";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@ReferenceType", referenceType);
                    cmd.Parameters.AddWithValue("@ReferenceId", referenceId);
                },
                CommandType.Text);
        }

        public async Task<List<long>> RestoreOrCreateAnnouncementNotificationsAsync(
            long announcementId,
            string title,
            string message)
        {
            const string restoreQuery = @"
                UPDATE tab_notifications
                SET
                    is_deleted = false,
                    title = @Title,
                    message = @Message
                WHERE reference_type = @ReferenceType
                  AND reference_id = @AnnouncementId
                  AND is_deleted = true";

            await ExecuteNonQueryAsync(
                restoreQuery,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Title", title ?? "");
                    cmd.Parameters.AddWithValue("@Message", message ?? "");
                    cmd.Parameters.AddWithValue("@ReferenceType", NotificationReferenceTypes.Announcement);
                    cmd.Parameters.AddWithValue("@AnnouncementId", announcementId);
                },
                CommandType.Text);

            const string insertQuery = @"
                INSERT INTO tab_notifications
                (
                    company_id,
                    user_id,
                    title,
                    message,
                    reference_type,
                    reference_id,
                    created_date,
                    is_read,
                    is_deleted,
                    toast_shown,
                    notification_count,
                    sender_user_id
                )
                SELECT
                    u.company_id,
                    u.user_id,
                    @Title,
                    @Message,
                    @ReferenceType,
                    @AnnouncementId,
                    NOW(),
                    false,
                    false,
                    false,
                    1,
                    NULL
                FROM tab_user_master u
                WHERE COALESCE(u.is_active, true) = true
                  AND u.company_id > 0
                  AND NOT EXISTS (
                      SELECT 1
                      FROM tab_notifications n
                      WHERE n.reference_type = @ReferenceType
                        AND n.reference_id = @AnnouncementId
                        AND n.user_id = u.user_id
                  )
                RETURNING user_id";

            return await ExecuteRawQueryAsync(
                insertQuery,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Title", title ?? "");
                    cmd.Parameters.AddWithValue("@Message", message ?? "");
                    cmd.Parameters.AddWithValue("@ReferenceType", NotificationReferenceTypes.Announcement);
                    cmd.Parameters.AddWithValue("@AnnouncementId", announcementId);
                },
                reader => Convert.ToInt64(reader["user_id"]));
        }
    }
}
