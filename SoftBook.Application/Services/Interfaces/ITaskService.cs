using SoftBook.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskModel>> GetTasks(int companyId);
        Task<TaskModel> GetTaskById(int companyId, long taskId);
        Task<long> AddTask(int companyId, TaskModel model);
        Task<bool> UpdateTask(int companyId, TaskModel model);
        Task<bool> DeleteTask(int companyId, long taskId);
    }
}
