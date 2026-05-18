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


            IsRecurring = reader["is_recurring"] == DBNull.Value
                ? false
                : Convert.ToBoolean(reader["is_recurring"]),

                        RecurringGroupId = reader["recurring_group_id"] == DBNull.Value
                ? null
                : Guid.Parse(reader["recurring_group_id"].ToString()),

                        RecurrenceParentId = reader["recurrence_parent_id"] == DBNull.Value
                ? null
                : Convert.ToInt64(reader["recurrence_parent_id"]),

                        RecurrenceType = reader["recurrence_type"]?.ToString(),

                        RecurrenceInterval = reader["recurrence_interval"] == DBNull.Value
                ? 1
                : Convert.ToInt32(reader["recurrence_interval"]),

                        RecurrenceDays = reader["recurrence_days"]?.ToString(),

                        RecurrenceRule = reader["recurrence_rule"]?.ToString(),

                        RecurrenceEndDate = reader["recurrence_end_date"] == DBNull.Value
                ? null
                : Convert.ToDateTime(reader["recurrence_end_date"]),

                        RecurrenceOccurrenceCount = reader["recurrence_occurrence_count"] == DBNull.Value
                ? null
                : Convert.ToInt32(reader["recurrence_occurrence_count"]),

                        ReminderOffsetMinutes = reader["reminder_offset_minutes"] == DBNull.Value
                ? null
                : Convert.ToInt32(reader["reminder_offset_minutes"]),
        };

        // ─── shared fn_tasks call builder ──────────────────────────────────────
        // private const string FnQuery = "SELECT * FROM fn_tasks(@Action, @TaskId, @Title, @Description, @InitiatedUserId, @AssignedUserId, @AssignedDate, @ExpectedDate, @Reminder, @ReminderDate, @FinishDate, @FinishDescription, @TaskStatus, @Priority, @TaskCategoryId,@CompanyId)";
        private const string FnQuery =
    "SELECT * FROM fn_tasks(" +
    "@Action," +
    "@TaskId," +
    "@Title," +
    "@Description," +
    "@InitiatedUserId," +
    "@AssignedUserId," +
    "@AssignedDate," +
    "@ExpectedDate," +
    "@Reminder," +
    "@ReminderDate," +
    "@FinishDate," +
    "@FinishDescription," +
    "@TaskStatus," +
    "@Priority," +
    "@TaskCategoryId," +
    "@IsRecurring," +
    "@RecurringGroupId," +
    "@RecurrenceParentId," +
    "@RecurrenceType," +
    "@RecurrenceInterval," +
    "@RecurrenceDays," +
    "@RecurrenceRule," +
    "@RecurrenceEndDate," +
    "@RecurrenceOccurrenceCount," +
    "@ReminderOffsetMinutes," +
    "@CompanyId" +
    ")";
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
            cmd.Parameters.Add(new NpgsqlParameter("@IsRecurring",
    NpgsqlTypes.NpgsqlDbType.Boolean)
            {
                Value = model?.IsRecurring ?? false
            });

            cmd.Parameters.Add(new NpgsqlParameter("@RecurringGroupId",
                NpgsqlTypes.NpgsqlDbType.Uuid)
            {
                Value = (object?)model?.RecurringGroupId ?? DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("@RecurrenceParentId",
                NpgsqlTypes.NpgsqlDbType.Bigint)
            {
                Value = (object?)model?.RecurrenceParentId ?? DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("@RecurrenceType",
                NpgsqlTypes.NpgsqlDbType.Text)
            {
                Value = (object?)model?.RecurrenceType ?? DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("@RecurrenceInterval",
                NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = model?.RecurrenceInterval ?? 1
            });

            cmd.Parameters.Add(new NpgsqlParameter("@RecurrenceDays",
                NpgsqlTypes.NpgsqlDbType.Text)
            {
                Value = (object?)model?.RecurrenceDays ?? DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("@RecurrenceRule",
                NpgsqlTypes.NpgsqlDbType.Text)
            {
                Value = (object?)model?.RecurrenceRule ?? DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("@RecurrenceEndDate",
                NpgsqlTypes.NpgsqlDbType.Timestamp)
            {
                Value = (object?)model?.RecurrenceEndDate ?? DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("@RecurrenceOccurrenceCount",
                NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = (object?)model?.RecurrenceOccurrenceCount ?? DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("@ReminderOffsetMinutes",
                NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = (object?)model?.ReminderOffsetMinutes ?? DBNull.Value
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

        public async Task<bool> UpdateTask(int companyId, TaskModel model)
        {
            await ExecuteNonQueryAsync(
                FnQuery,
                cmd => BindParams(cmd, "UPDATE", companyId, model),
                CommandType.Text);

            // If reminder is enabled, reset reminder_sent so the background service
            // re-evaluates the (possibly changed) reminder date/time.
            if (model.Reminder && model.ReminderDate.HasValue)
            {
                string resetQuery = @"
    UPDATE tab_tasks
    SET reminder_sent = false
    WHERE task_id = @TaskId
    AND company_id = @CompanyId";

                await ExecuteNonQueryAsync(
                    resetQuery,
                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("@TaskId", model.TaskId);
                        cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    },
                    CommandType.Text);
            }

            return true;
        }

        public async Task<bool> DeleteTask(int companyId,long taskId)
        {
            string query = @"
                UPDATE tab_tasks
                SET is_deleted = true
                WHERE company_id = @CompanyId
                AND task_id = @TaskId
                AND task_status NOT IN ('Completed', 'Canceled')";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                },
                CommandType.Text);

            return true;
        }

        public async Task<List<TaskModel>> GetTasksByGroupId(int companyId, Guid groupId)
        {
            string query = @"
                SELECT *
                FROM tab_tasks
                WHERE company_id = @CompanyId
                AND recurring_group_id = @GroupId
                AND is_deleted = false";

            return await ExecuteRawQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                    cmd.Parameters.Add(new NpgsqlParameter("@GroupId", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = groupId });
                },
                MapReader);
        }

        public async Task<bool> DeleteFutureTasks(int companyId, Guid groupId, DateTime fromDate)
        {
            string query = @"
                UPDATE tab_tasks
                SET is_deleted = true
                WHERE company_id = @CompanyId
                AND recurring_group_id = @GroupId
                AND assigned_date >= @FromDate
                AND task_status NOT IN ('Completed', 'Canceled')";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                    cmd.Parameters.Add(new NpgsqlParameter("@GroupId", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = groupId });
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                },
                CommandType.Text);

            return true;
        }

        public async Task<bool> DeleteTaskSeries(int companyId, Guid groupId)
        {
            string query = @"
                UPDATE tab_tasks
                SET is_deleted = true
                WHERE company_id = @CompanyId
                AND recurring_group_id = @GroupId
                AND task_status NOT IN ('Completed', 'Canceled')";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", (long)companyId);
                    cmd.Parameters.Add(new NpgsqlParameter("@GroupId", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = groupId });
                },
                CommandType.Text);

            return true;
        }

        public async Task<List<TaskModel>> GetPendingReminders(int companyId)
        {
            string query = @"
    SELECT *
    FROM tab_tasks
    WHERE
        reminder = true
        AND reminder_sent = false
        AND reminder_date <= NOW()
        AND is_deleted = false
        AND task_status NOT IN ('Completed', 'Canceled')
        AND company_id = @CompanyId
";

            return await ExecuteRawQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                },
                MapReader);
        }

        // GetAllPendingReminders() removed - architecture mismatch (DB-per-tenant)

        public async Task MarkReminderSent(
    int companyId,
    long taskId)
        {
            string query = @"
    UPDATE tab_tasks
    SET reminder_sent = true
    WHERE task_id = @TaskId
    AND company_id = @CompanyId
";

            await ExecuteNonQueryAsync(
                query,
                cmd =>
                {
                    cmd.Parameters.AddWithValue(
                        "@TaskId",
                        taskId);

                    cmd.Parameters.AddWithValue(
                        "@CompanyId",
                        companyId);
                },
                CommandType.Text);
        }

    }
}
