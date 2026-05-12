using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoftBook.API.Services
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReminderBackgroundService> _logger;

        public ReminderBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Reminder Background Service Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation(
                        "Checking reminders...");

                    using (var scope =
                        _serviceProvider.CreateScope())
                    {
                        var configuration =
                            scope.ServiceProvider
                                 .GetRequiredService<IConfiguration>();

                        var connectionString =
                            configuration.GetConnectionString(
                                "LocalConnection");

                        using var connection =
                            new NpgsqlConnection(connectionString);

                        await connection.OpenAsync(stoppingToken);

                        // STEP 1: Find pending reminders
                        string selectQuery = @"
                            SELECT
                                task_id,
                                company_id,
                                assigned_user_id,
                                title
                            FROM tab_tasks
                            WHERE
                                reminder = true
                                AND reminder_sent = false
                                AND reminder_date <= NOW()
                                AND is_deleted = false
                                AND task_status != 'Completed'
                        ";

                        using var selectCommand =
                            new NpgsqlCommand(
                                selectQuery,
                                connection);

                        using var reader =
                            await selectCommand
                                .ExecuteReaderAsync(stoppingToken);

                        var tasks = new List<(
                            long TaskId,
                            long CompanyId,
                            long UserId,
                            string Title)>();

                        while (await reader.ReadAsync(stoppingToken))
                        {
                            tasks.Add((
                                Convert.ToInt64(reader["task_id"]),
                                Convert.ToInt64(reader["company_id"]),
                                reader["assigned_user_id"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt64(
                                        reader["assigned_user_id"]),
                                reader["title"].ToString()
                            ));
                        }

                        await reader.CloseAsync();

                        // STEP 2: Create notifications
                        foreach (var task in tasks)
                        {
                            _logger.LogInformation(
                                $"Reminder Found: {task.TaskId}");

                            string insertNotification = @"
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
                                    is_deleted
                                )
                                VALUES
                                (
                                    @CompanyId,
                                    @UserId,
                                    @Title,
                                    @Message,
                                    'TASK',
                                    @ReferenceId,
                                    NOW(),
                                    false,
                                    false
                                )
                            ";

                            using var insertCommand =
                                new NpgsqlCommand(
                                    insertNotification,
                                    connection);

                            insertCommand.Parameters.AddWithValue(
                                "@CompanyId",
                                task.CompanyId);

                            insertCommand.Parameters.AddWithValue(
                                "@UserId",
                                task.UserId);

                            insertCommand.Parameters.AddWithValue(
                                "@Title",
                                "Task Reminder");

                            insertCommand.Parameters.AddWithValue(
                                "@Message",
                                $"Reminder for task: {task.Title}");

                            insertCommand.Parameters.AddWithValue(
                                "@ReferenceId",
                                task.TaskId);

                            await insertCommand
                                .ExecuteNonQueryAsync(stoppingToken);

                            // STEP 3: Mark reminder as sent
                            string updateTask = @"
                                UPDATE tab_tasks
                                SET reminder_sent = true
                                WHERE task_id = @TaskId
                            ";

                            using var updateCommand =
                                new NpgsqlCommand(
                                    updateTask,
                                    connection);

                            updateCommand.Parameters.AddWithValue(
                                "@TaskId",
                                task.TaskId);

                            await updateCommand
                                .ExecuteNonQueryAsync(stoppingToken);

                            _logger.LogInformation(
                                $"Notification created for task {task.TaskId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error in ReminderBackgroundService");
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }
        }
    }
}