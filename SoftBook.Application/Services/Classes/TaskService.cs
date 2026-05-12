using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Application.Services.Classes
{
    /// <summary>
    /// Service layer responsible for business logic and orchestrating repository calls.
    /// Acts as a bridge between the Controller and Repository.
    /// </summary>
    public class TaskService : ITaskService
    {

        private readonly ITaskRepository _taskRepository;
        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<TaskModel> GetTaskById(int companyId,long taskId)
        {
            return await _taskRepository.GetTaskById(companyId,taskId);
        }

        public async Task<long> AddTask(int companyId, TaskModel model)
        {
            return await _taskRepository.AddTask(companyId, model);
        }

        public async Task<bool> DeleteTask(int companyId, long taskId)
        {
            return await _taskRepository.DeleteTask(companyId,taskId);
        }

        public async Task<List<TaskModel>> GetTasks(int companyId)
        {
            return await _taskRepository.GetTasks(companyId);
        }

        public async Task<bool> UpdateTask(int companyId, TaskModel model)
        {
            return await _taskRepository.UpdateTask(companyId, model);
        }

    }
}
