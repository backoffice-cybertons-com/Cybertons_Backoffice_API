using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SoftBook.Infrastructure.Classes
{
    public class AnnouncementRepository : BaseDAL, IAnnouncementRepository
    {
        public AnnouncementRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        private static AnnouncementModel MapReader(IDataReader reader)
        {
            var deleteFlag = false;
            try
            {
                // Column may not exist in every query result set
                var raw = reader["delete_flag"];
                if (raw != DBNull.Value && raw != null)
                    deleteFlag = Convert.ToBoolean(raw);
            }
            catch { /* column absent – leave false */ }

            var status = reader["status"]?.ToString();

            // Ensure the frontend always receives the correct status for
            // soft-deleted rows, regardless of what the DB function returns.
            if (deleteFlag && !string.Equals(status, "deleted", StringComparison.OrdinalIgnoreCase))
                status = "deleted";

            return new AnnouncementModel
            {
                AnnouncementId = Convert.ToInt64(reader["announcement_id"]),
                Title          = reader["title"]?.ToString(),
                ShortMessage   = reader["short_message"]?.ToString(),
                FullMessage    = reader["full_message"]?.ToString(),
                YoutubeUrl     = reader["youtube_url"]?.ToString(),
                ButtonText     = reader["button_text"]?.ToString(),
                ButtonLink     = reader["button_link"]?.ToString(),

                StartsAt = reader["starts_at"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["starts_at"]),

                ExpiresAt = reader["expires_at"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["expires_at"]),

                IsActive = reader["is_active"] != DBNull.Value
                    && Convert.ToBoolean(reader["is_active"]),

                CreatedAt = reader["created_at"] == DBNull.Value
                    ? DateTime.UtcNow
                    : Convert.ToDateTime(reader["created_at"]),

                DeleteFlag = deleteFlag,
                Status     = status,
            };
        }

        public async Task<long> CreateAsync(CreateAnnouncementRequest request)
        {
            const string query = @"
                SELECT announcement_id 
                FROM fn_manage_announcements(
                    p_action_type => 'CREATE',
                    p_title => @Title,
                    p_short_message => @ShortMessage,
                    p_full_message => @FullMessage,
                    p_youtube_url => @YoutubeUrl,
                    p_button_text => @ButtonText,
                    p_button_link => @ButtonLink,
                    p_starts_at => @StartsAt,
                    p_expires_at => @ExpiresAt,
                    p_is_active => @IsActive
                )";

            return await ExecuteScalarAsync<long>(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Title", request.Title ?? "");
                    cmd.Parameters.AddWithValue("@ShortMessage", (object?)request.ShortMessage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FullMessage", (object?)request.FullMessage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@YoutubeUrl", (object?)request.YoutubeUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ButtonText", (object?)request.ButtonText ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ButtonLink", (object?)request.ButtonLink ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartsAt", request.StartsAt!.Value.ToUniversalTime());
                    cmd.Parameters.AddWithValue("@ExpiresAt", request.ExpiresAt!.Value.ToUniversalTime());
                    cmd.Parameters.AddWithValue("@IsActive", request.IsActive);
                },
                CommandType.Text);
        }

        public async Task<AnnouncementModel?> GetByIdAsync(long announcementId)
        {
            const string query = @"
                SELECT *
                FROM fn_manage_announcements(
                    p_action_type => 'GET_BY_ID',
                    p_announcement_id => @AnnouncementId
                )
                LIMIT 1";

            var results = await ExecuteRawQueryAsync(
                query,
                cmd => cmd.Parameters.AddWithValue("@AnnouncementId", announcementId),
                MapReader);

            return results.FirstOrDefault();
        }

        public async Task<List<AnnouncementModel>> GetAllAsync()
        {
            // Query the table directly so that delete_flag is always present in
            // the result set (fn_manage_announcements GET_ALL does not expose it).
            const string query = @"
                SELECT
                    announcement_id,
                    title,
                    short_message,
                    full_message,
                    youtube_url,
                    button_text,
                    button_link,
                    starts_at,
                    expires_at,
                    is_active,
                    created_at,
                    is_deleted,
                    CASE
                        WHEN is_deleted THEN 'deleted'
                        WHEN expires_at < NOW()   THEN 'expired'
                        WHEN starts_at  > NOW()   THEN 'scheduled'
                        WHEN is_active            THEN 'active'
                        ELSE                           'inactive'
                    END AS status
                FROM tab_announcements
                ORDER BY created_at DESC";

            return await ExecuteRawQueryAsync(query, _ => { }, MapReader);
        }

        public async Task<bool> UpdateAsync(long announcementId, UpdateAnnouncementRequest request)
        {
            const string query = @"
                SELECT announcement_id 
                FROM fn_manage_announcements(
                    p_action_type => 'UPDATE',
                    p_announcement_id => @AnnouncementId,
                    p_title => @Title,
                    p_short_message => @ShortMessage,
                    p_full_message => @FullMessage,
                    p_youtube_url => @YoutubeUrl,
                    p_button_text => @ButtonText,
                    p_button_link => @ButtonLink,
                    p_starts_at => @StartsAt,
                    p_expires_at => @ExpiresAt,
                    p_is_active => @IsActive
                )";

            var updatedId = await ExecuteScalarAsync<long>(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AnnouncementId", announcementId);
                    cmd.Parameters.AddWithValue("@Title", request.Title ?? "");
                    cmd.Parameters.AddWithValue("@ShortMessage", (object?)request.ShortMessage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FullMessage", (object?)request.FullMessage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@YoutubeUrl", (object?)request.YoutubeUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ButtonText", (object?)request.ButtonText ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ButtonLink", (object?)request.ButtonLink ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartsAt", request.StartsAt!.Value.ToUniversalTime());
                    cmd.Parameters.AddWithValue("@ExpiresAt", request.ExpiresAt!.Value.ToUniversalTime());
                    cmd.Parameters.AddWithValue("@IsActive", request.IsActive);
                },
                CommandType.Text);

            return updatedId > 0;
        }

        public async Task DeleteAsync(long announcementId)
        {
            const string query = @"
                SELECT announcement_id 
                FROM fn_manage_announcements(
                    p_action_type => 'DELETE',
                    p_announcement_id => @AnnouncementId
                )";

            await ExecuteScalarAsync<long>(
                query,
                cmd => cmd.Parameters.AddWithValue("@AnnouncementId", announcementId),
                CommandType.Text);
        }

        public async Task<bool> SetActiveAsync(long announcementId, bool isActive)
        {
            const string query = @"
                SELECT announcement_id 
                FROM fn_manage_announcements(
                    p_action_type => 'SET_ACTIVE',
                    p_announcement_id => @AnnouncementId,
                    p_is_active => @IsActive
                )";

            var updatedId = await ExecuteScalarAsync<long>(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AnnouncementId", announcementId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                },
                CommandType.Text);

            return updatedId > 0;
        }

        public async Task<AnnouncementModel?> GetLatestUnseenAsync(long userId)
        {
            const string query = @"
                SELECT *
                FROM fn_manage_announcements(
                    p_action_type => 'GET_LATEST_UNSEEN',
                    p_user_id => @UserId
                )
                LIMIT 1";

            var results = await ExecuteRawQueryAsync(
                query,
                cmd => cmd.Parameters.AddWithValue("@UserId", userId),
                MapReader);

            return results.FirstOrDefault();
        }

        public async Task MarkAsSeenAsync(long announcementId, long userId)
        {
            const string query = @"
                SELECT 1
                FROM fn_manage_announcements(
                    p_action_type => 'MARK_SEEN',
                    p_announcement_id => @AnnouncementId,
                    p_user_id => @UserId
                )";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@AnnouncementId", announcementId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                },
                CommandType.Text);
        }
    }
}
