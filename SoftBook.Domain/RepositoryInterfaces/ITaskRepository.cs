using SoftBook.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskModel>> GetTasks(int companyId);
        Task<TaskModel> GetTaskById(int companyId, long taskId);
        Task<long> AddTask(int companyId, TaskModel model);
        Task<bool> UpdateTask(int companyId, TaskModel model);
        Task<bool> DeleteTask(int companyId,long taskId);
        Task<List<TaskModel>> GetTasksByGroupId(int companyId, Guid groupId);
        Task<bool> DeleteFutureTasks(int companyId, Guid groupId, DateTime fromDate);
        Task<bool> DeleteTaskSeries(int companyId, Guid groupId);

        /// <summary>
        /// Per-company reminder fetch (used by API controllers if needed).
        /// </summary>
        Task<List<TaskModel>> GetPendingReminders(int companyId);



        Task MarkReminderSent(int companyId, long taskId);
    }
}
