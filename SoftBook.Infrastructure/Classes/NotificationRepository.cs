using Microsoft.Extensions.Configuration;
using Npgsql;
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
            ToastShown = Convert.ToBoolean(reader["toast_shown"])
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
                WHERE notification_id = @NotificationId";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@NotificationId", notificationId);
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
                    toast_shown
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
                    false
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
                },
                CommandType.Text);
        }

        public async Task<bool> MarkToastAsShown(long companyId, long notificationId)
        {
            string query = @"
                UPDATE tab_notifications 
                SET toast_shown = true 
                WHERE notification_id = @NotificationId";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@NotificationId", notificationId);
                },
                CommandType.Text);

            return true;
        }
    }
}
