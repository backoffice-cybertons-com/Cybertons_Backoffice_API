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
    public class SystemLogRepository : BaseDAL, ISystemLogRepository
    {
        public SystemLogRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        public async Task InsertAsync(
            int companyId,
            int userId,
            string actionType,
            string? moduleName = null,
            int? recordId = null,
            string? logMessage = null)
        {
            var message = logMessage;
            if (string.IsNullOrWhiteSpace(message))
            {
                message = recordId.HasValue
                    ? $"{actionType} record {recordId.Value}"
                    : actionType;
            }

            await ExecuteNonQueryAsync(@"
                INSERT INTO public.tab_system_log (
                    log_time, module_name, message, user_id, action_type, company_id
                ) VALUES (
                    CURRENT_TIMESTAMP, @p_module_name, @p_message, @p_user_id, @p_action_type, @p_company_id
                )",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_module_name", (object?)moduleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_message", (object?)message ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_user_id", userId);
                    cmd.Parameters.AddWithValue("p_action_type", actionType);
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                },
                CommandType.Text);
        }

        public async Task<List<SystemLogDatewiseSummaryDto>> GetDatewiseSummaryAsync(
            int companyId,
            DateTime fromDate,
            DateTime toDate)
        {
            return await ExecuteReaderAsync(@"
                SELECT
                    sl.user_id,
                    COALESCE(MAX(u.username), MAX(u.full_name), '') AS username,
                    COALESCE(SUM(CASE WHEN UPPER(TRIM(sl.action_type)) = 'LOGIN' THEN 1 ELSE 0 END), 0) AS login_count,
                    COALESCE(SUM(CASE WHEN UPPER(TRIM(sl.action_type)) = 'LOGOUT' THEN 1 ELSE 0 END), 0) AS logout_count,
                    COALESCE(SUM(CASE
                        WHEN UPPER(TRIM(sl.action_type)) = 'ADD'
                         AND LOWER(TRIM(COALESCE(sl.module_name, ''))) = 'tab_sale_pur'
                        THEN 1 ELSE 0 END), 0) AS add_count,
                    COALESCE(SUM(CASE
                        WHEN UPPER(TRIM(sl.action_type)) = 'EDIT'
                         AND LOWER(TRIM(COALESCE(sl.module_name, ''))) = 'tab_sale_pur'
                        THEN 1 ELSE 0 END), 0) AS edit_count,
                    COALESCE(SUM(CASE
                        WHEN UPPER(TRIM(sl.action_type)) = 'DELETE'
                         AND LOWER(TRIM(COALESCE(sl.module_name, ''))) = 'tab_sale_pur'
                        THEN 1 ELSE 0 END), 0) AS delete_count
                FROM public.tab_system_log sl
                LEFT JOIN public.tab_user_master u
                    ON u.user_id = sl.user_id
                   AND u.company_id = @p_company_id
                WHERE sl.company_id = @p_company_id
                  AND sl.log_time::date BETWEEN @p_from_date AND @p_to_date
                  AND sl.user_id IS NOT NULL
                GROUP BY sl.user_id
                ORDER BY username",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                    cmd.Parameters.Add(new NpgsqlParameter("p_from_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = fromDate.Date });
                    cmd.Parameters.Add(new NpgsqlParameter("p_to_date", NpgsqlTypes.NpgsqlDbType.Date) { Value = toDate.Date });
                },
                reader => new SystemLogDatewiseSummaryDto
                {
                    UserId = Convert.ToInt32(reader["user_id"]),
                    Username = reader["username"]?.ToString() ?? string.Empty,
                    LoginCount = Convert.ToInt32(reader["login_count"]),
                    LogoutCount = Convert.ToInt32(reader["logout_count"]),
                    AddCount = Convert.ToInt32(reader["add_count"]),
                    EditCount = Convert.ToInt32(reader["edit_count"]),
                    DeleteCount = Convert.ToInt32(reader["delete_count"])
                },
                CommandType.Text);
        }
    }
}
