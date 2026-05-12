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
    public class TaskTemplateRepository : BaseDAL, ITaskTemplateRepository
    {
        public TaskTemplateRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        private const string FnQuery =
    "SELECT * FROM fn_task_templates(@Action, @TemplateId, @TemplateName, @Description, @DefaultPriority, @DefaultStatus, @DefaultAssignedUserId, @TemplateColor, @CompanyId)";
        private static TaskTemplateModel MapReader(IDataReader reader) =>
    new TaskTemplateModel
    {
        TemplateId = Convert.ToInt64(reader["template_id"]),
        TemplateName = reader["template_name"]?.ToString(),
        Description = reader["description"]?.ToString(),
        DefaultPriority = reader["default_priority"]?.ToString(),
        DefaultStatus = reader["default_status"]?.ToString(),
        DefaultAssignedUserId =
            reader["default_assigned_user_id"] == DBNull.Value
                ? null
                : Convert.ToInt64(reader["default_assigned_user_id"]),
        TemplateColor = reader["template_color"]?.ToString(),
        CreatedDate = Convert.ToDateTime(reader["created_date"]),
        UpdatedDate =
            reader["updated_date"] == DBNull.Value
                ? null
                : Convert.ToDateTime(reader["updated_date"]),
        IsDeleted = Convert.ToBoolean(reader["is_deleted"]),
        CompanyId = Convert.ToInt64(reader["company_id"])
    };

        private static void BindParams(
    IDbCommand cmd,
    string action,
    long? companyId = null,
    TaskTemplateModel model = null,
    long? templateId = null)
        {
            cmd.Parameters.Add(
                new NpgsqlParameter("@Action", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = action });

            cmd.Parameters.Add(
                new NpgsqlParameter("@TemplateId", NpgsqlTypes.NpgsqlDbType.Bigint)
                { Value = (object)(templateId ?? model?.TemplateId) ?? DBNull.Value });

            cmd.Parameters.Add(
                new NpgsqlParameter("@TemplateName", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = (object)model?.TemplateName ?? DBNull.Value });

            cmd.Parameters.Add(
                new NpgsqlParameter("@Description", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = (object)model?.Description ?? DBNull.Value });

            cmd.Parameters.Add(
                new NpgsqlParameter("@DefaultPriority", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = (object)model?.DefaultPriority ?? DBNull.Value });

            cmd.Parameters.Add(
                new NpgsqlParameter("@DefaultStatus", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = (object)model?.DefaultStatus ?? DBNull.Value });

            cmd.Parameters.Add(
                new NpgsqlParameter("@DefaultAssignedUserId", NpgsqlTypes.NpgsqlDbType.Bigint)
                { Value = (object)model?.DefaultAssignedUserId ?? DBNull.Value });

            cmd.Parameters.Add(
                new NpgsqlParameter("@TemplateColor", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = (object)model?.TemplateColor ?? DBNull.Value });

            cmd.Parameters.Add(
                new NpgsqlParameter("@CompanyId", NpgsqlTypes.NpgsqlDbType.Bigint)
                { Value = (object)(companyId ?? model?.CompanyId) ?? DBNull.Value });
        }

        public async Task<List<TaskTemplateModel>> GetTemplates(int companyId)
        {
            return await ExecuteRawQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "GETALL", companyId),
                MapReader);
        }
        public async Task<TaskTemplateModel> GetTemplateById(int companyId, long templateId)
        {
            var results = await ExecuteRawQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "GETBYID", companyId, templateId: templateId),
                MapReader);

            return results.FirstOrDefault();
        }
        public async Task<long> AddTemplate(int companyId, TaskTemplateModel model)
        {
            var results = await ExecuteRawQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "INSERT", companyId, model),
                MapReader);

            return results.FirstOrDefault()?.TemplateId ?? 0;
        }
        public async Task<bool> UpdateTemplate(int companyId, TaskTemplateModel model)
        {
            await ExecuteNonQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "UPDATE", companyId, model),
                CommandType.Text);

            return true;
        }
        public async Task<bool> DeleteTemplate(int companyId, long templateId)
        {
            await ExecuteNonQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "DELETE", companyId, templateId: templateId),
                CommandType.Text);

            return true;
        }
    }
}
