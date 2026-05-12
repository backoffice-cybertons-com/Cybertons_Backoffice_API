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
    /// <summary>
    /// Repository responsible for database interactions with PostgreSQL tab_tasks table.
    /// All operations are routed through the fn_tasks stored function using p_action parameter.
    /// </summary>
    public class TaskRepository : BaseDAL, ITaskRepository
    {
        public TaskRepository(IConfiguration configuration, ITenantProvider tenantProvider)
            : base(configuration, tenantProvider)
        {
        }

        // ─── shared reader mapper ───────────────────────────────────────────────
        private static TaskModel MapReader(IDataReader reader) => new TaskModel
        {
            TaskId           = Convert.ToInt64(reader["task_id"]),
            Title            = reader["title"]?.ToString(),
            Description      = reader["description"]?.ToString(),
            InitiatedUserId  = Convert.ToInt64(reader["initiated_user_id"]),
            AssignedUserId   = reader["assigned_user_id"]  == DBNull.Value ? null : Convert.ToInt64(reader["assigned_user_id"]),
            AssignedDate     = reader["assigned_date"]     == DBNull.Value ? null : Convert.ToDateTime(reader["assigned_date"]),
            ExpectedDate     = reader["expected_date"]     == DBNull.Value ? null : Convert.ToDateTime(reader["expected_date"]),
            Reminder         = Convert.ToBoolean(reader["reminder"]),
            ReminderDate     = reader["reminder_date"]     == DBNull.Value ? null : Convert.ToDateTime(reader["reminder_date"]),
            FinishDate       = reader["finish_date"]       == DBNull.Value ? null : Convert.ToDateTime(reader["finish_date"]),
            FinishDescription= reader["finish_description"]?.ToString(),
            TaskStatus       = reader["task_status"]?.ToString(),
            Priority         = reader["priority"]?.ToString(),
            TaskCategoryId   = reader["task_category_id"] == DBNull.Value ? null : Convert.ToInt64(reader["task_category_id"]),
            IsDeleted        = Convert.ToBoolean(reader["is_deleted"]),
            CompanyId = Convert.ToInt64(reader["company_id"]),
        };

        // ─── shared fn_tasks call builder ──────────────────────────────────────
        private const string FnQuery = "SELECT * FROM fn_tasks(@Action, @TaskId, @Title, @Description, @InitiatedUserId, @AssignedUserId, @AssignedDate, @ExpectedDate, @Reminder, @ReminderDate, @FinishDate, @FinishDescription, @TaskStatus, @Priority, @TaskCategoryId,@CompanyId)";

        private static void BindParams(
            IDbCommand      cmd,
            string          action,
            long? companyId = null,
            TaskModel model   = null,
            long?           taskId  = null)
        {
            cmd.Parameters.Add(new NpgsqlParameter("@Action",          NpgsqlTypes.NpgsqlDbType.Text)      { Value = action });
            cmd.Parameters.Add(new NpgsqlParameter("@TaskId",          NpgsqlTypes.NpgsqlDbType.Bigint)    { Value = (object)(taskId ?? model?.TaskId) ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@Title",           NpgsqlTypes.NpgsqlDbType.Text)      { Value = (object)model?.Title              ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@Description",     NpgsqlTypes.NpgsqlDbType.Text)      { Value = (object)model?.Description        ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@InitiatedUserId", NpgsqlTypes.NpgsqlDbType.Bigint)    { Value = (object)(model?.InitiatedUserId)   ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@AssignedUserId",  NpgsqlTypes.NpgsqlDbType.Bigint)    { Value = (object)model?.AssignedUserId      ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@AssignedDate",    NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = (object)model?.AssignedDate        ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@ExpectedDate",    NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = (object)model?.ExpectedDate        ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@Reminder",        NpgsqlTypes.NpgsqlDbType.Boolean)   { Value = model?.Reminder ?? false });
            cmd.Parameters.Add(new NpgsqlParameter("@ReminderDate",    NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = (object)model?.ReminderDate        ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@FinishDate",      NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = (object)model?.FinishDate          ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@FinishDescription",NpgsqlTypes.NpgsqlDbType.Text)     { Value = (object)model?.FinishDescription   ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@TaskStatus",      NpgsqlTypes.NpgsqlDbType.Text)      { Value = (object)model?.TaskStatus          ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@Priority",        NpgsqlTypes.NpgsqlDbType.Text)      { Value = (object)model?.Priority            ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("@TaskCategoryId",  NpgsqlTypes.NpgsqlDbType.Bigint)    { Value = (object)model?.TaskCategoryId      ?? DBNull.Value });
            cmd.Parameters.Add(
                new NpgsqlParameter("@CompanyId", NpgsqlTypes.NpgsqlDbType.Bigint)
                {
                    Value = (object)(companyId ?? model?.CompanyId) ?? DBNull.Value
                });
        }

        // ─── public methods ─────────────────────────────────────────────────────

        public async Task<List<TaskModel>> GetTasks(int companyId)
        {
            return await ExecuteRawQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "GETALL",companyId),
                MapReader);
        }

        public async Task<TaskModel> GetTaskById(int companyId,long taskId)
        {
            var results = await ExecuteRawQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "GETBYID", companyId, taskId: taskId),
                MapReader);

            return results.FirstOrDefault();
        }

        public async Task<long> AddTask(int companyId,TaskModel model)
        {
            var results = await ExecuteRawQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "INSERT", companyId,model),
                MapReader);

            return results.FirstOrDefault()?.TaskId ?? 0;
        }

        public async Task<bool> UpdateTask(int companyId,TaskModel model)
        {
            await ExecuteNonQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "UPDATE", companyId, model),
                CommandType.Text);

            return true;
        }

        public async Task<bool> DeleteTask(int companyId,long taskId)
        {
            await ExecuteNonQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "DELETE", companyId,taskId: taskId),
                CommandType.Text);

            return true;
        }

       
    }
}
