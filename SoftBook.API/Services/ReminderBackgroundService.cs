using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoftBook.API.Services
{
    /// <summary>
    /// Hosted background service that polls CompanyMaster for all tenants,
    /// then switches database connections to each tenant's specific database
    /// to process task reminders.
    ///
    /// Flow per cycle:
    ///   1. Connect to CompanyMaster -> Fetch all active companies.
    ///   2. Loop through each company.
    ///   3. Switch connection context (ITenantProvider) to the tenant's DB.
    ///   4. Fetch pending reminders for that tenant.
    ///   5. Process notifications and mark as sent.
    /// </summary>
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReminderBackgroundService> _logger;

        // How long to wait between reminder-check cycles.
        private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(1);

        public ReminderBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger         = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "[ReminderService] Started. Polling interval: {Interval}.",
                PollingInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAllPendingReminders(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "[ReminderService] Unhandled error in polling cycle. Will retry next interval.");
                }

                await Task.Delay(PollingInterval, stoppingToken);
            }

            _logger.LogInformation("[ReminderService] Stopped.");
        }

        private async Task ProcessAllPendingReminders(CancellationToken stoppingToken)
        {
            using var globalScope = _serviceProvider.CreateScope();
            var companyRepo = globalScope.ServiceProvider.GetRequiredService<ICompanyRepository>();

            _logger.LogInformation("========== Reminder Service Cycle Started (DB-per-Tenant) ==========");

            // 1. Get all active companies from CompanyMaster
            var companies = await companyRepo.GetAllAsync();
            _logger.LogInformation("[ReminderService] Found {Count} active companies to process.", companies.Count);

            foreach (var company in companies)
            {
                if (stoppingToken.IsCancellationRequested) break;

                _logger.LogInformation(">>> Processing Company: {CompanyName} (ID: {CompanyId}) | DB: {DbName}", 
                    company.Name, company.Id, GetDbNameFromConnectionString(company.ConnectionString));

                try
                {
                    // Create a new scope for this tenant to ensure fresh repositories with correct connection string
                    using var tenantScope = _serviceProvider.CreateScope();
                    var tenantProvider = tenantScope.ServiceProvider.GetRequiredService<ITenantProvider>();
                    
                    // IMPORTANT: Switch the connection string for this tenant
                    tenantProvider.ConnectionString = company.ConnectionString;
                    tenantProvider.CompanyId = company.Id;

                    var taskRepo = tenantScope.ServiceProvider.GetRequiredService<ITaskRepository>();
                    var notificationRepo = tenantScope.ServiceProvider.GetRequiredService<INotificationRepository>();

                    // 2. Fetch pending reminders for THIS tenant only
                    var pendingReminders = await taskRepo.GetPendingReminders(company.Id);

                    _logger.LogInformation("[ReminderService]   - Found {Count} reminders for tenant {CompanyName}.", 
                        pendingReminders.Count, company.Name);

                    if (pendingReminders.Count == 0) continue;

                    int successCount = 0;
                    int failCount = 0;

                    foreach (var task in pendingReminders)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        if (!task.AssignedUserId.HasValue)
                        {
                            _logger.LogWarning("[ReminderService]     - Task {TaskId} has no assigned user — skipping.", task.TaskId);
                            continue;
                        }

                        try
                        {
                            var notification = new NotificationModel
                            {
                                CompanyId = task.CompanyId,
                                UserId = task.AssignedUserId.Value,
                                Title = "Task Reminder",
                                Message = $"Reminder: \"{task.Title}\" is due.",
                                ReferenceType = "TASK",
                                ReferenceId = task.TaskId,
                                CreatedDate = DateTime.UtcNow
                            };

                            await notificationRepo.AddNotification(notification);
                            await taskRepo.MarkReminderSent((int)task.CompanyId, task.TaskId);

                            _logger.LogInformation("[ReminderService]     - Reminder sent: Task {TaskId} -> User {UserId}", 
                                task.TaskId, task.AssignedUserId.Value);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            _logger.LogError(ex, "[ReminderService]     - Error processing Task {TaskId} for {CompanyName}", 
                                task.TaskId, company.Name);
                        }
                    }

                    _logger.LogInformation("[ReminderService]   - {CompanyName} summary: {Success} sent, {Fail} failed.", 
                        company.Name, successCount, failCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[ReminderService] Critical error processing tenant {CompanyName} (ID: {CompanyId}). Skipping to next tenant.", 
                        company.Name, company.Id);
                }
            }

            _logger.LogInformation("========== Reminder Service Cycle Complete ==========");
        }

        private string GetDbNameFromConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return "NULL";
            try
            {
                var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
                return builder.Database ?? "Unknown";
            }
            catch
            {
                return "Invalid Connection String";
            }
        }
    }
}