using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IRecurrenceService _recurrenceService;

        public TaskService(ITaskRepository taskRepository, IRecurrenceService recurrenceService)
        {
            _taskRepository = taskRepository;
            _recurrenceService = recurrenceService;
        }
        public async Task<TaskModel> GetTaskById(int companyId,long taskId)
        {
            return await _taskRepository.GetTaskById(companyId,taskId);
        }

        public async Task<long> AddTask(int companyId, TaskModel model)
        {
            // 1. Assign RecurringGroupId before saving if recurring
            if (model.IsRecurring && (model.RecurringGroupId == null || model.RecurringGroupId == Guid.Empty))
            {
                model.RecurringGroupId = Guid.NewGuid();
            }

            // Handle Reminders dynamically based on assigned date and offset for the Parent Task
            if (model.Reminder && model.AssignedDate.HasValue && model.ReminderOffsetMinutes.HasValue)
            {
                model.ReminderDate = model.AssignedDate.Value.AddMinutes(model.ReminderOffsetMinutes.Value);
            }

            // 2. Save parent task first
            long parentTaskId = await _taskRepository.AddTask(companyId, model);
            model.TaskId = parentTaskId;

            // 3. Generate and save occurrences if recurring (only if it's the root parent)
            if (model.IsRecurring && parentTaskId > 0 && model.RecurrenceParentId == null)
            {
                var occurrences = _recurrenceService.GenerateOccurrences(model);
                foreach (var occ in occurrences)
                {
                    await _taskRepository.AddTask(companyId, occ);
                }
            }

            return parentTaskId;
        }

        public async Task<bool> DeleteTask(int companyId, long taskId, string deleteType = "this")
        {
            var task = await _taskRepository.GetTaskById(companyId, taskId);
            if (task == null) return false;

            if (deleteType == "this" || task.RecurringGroupId == null)
            {
                if (task.TaskStatus == "Completed" || task.TaskStatus == "Canceled") return false;
                return await _taskRepository.DeleteTask(companyId, taskId);
            }

            if (deleteType == "all")
            {
                return await _taskRepository.DeleteTaskSeries(companyId, task.RecurringGroupId.Value);
            }

            if (deleteType == "future" && task.AssignedDate.HasValue)
            {
                return await _taskRepository.DeleteFutureTasks(companyId, task.RecurringGroupId.Value, task.AssignedDate.Value);
            }

            if (task.TaskStatus == "Completed" || task.TaskStatus == "Canceled") return false;
            return await _taskRepository.DeleteTask(companyId, taskId);
        }

        public async Task<List<TaskModel>> GetTasks(int companyId)
        {
            return await _taskRepository.GetTasks(companyId);
        }

        public async Task<bool> UpdateTask(int companyId, TaskModel model, string updateType = "this")
        {
            // Handle Reminders dynamically based on assigned date and offset
            if (model.Reminder && model.AssignedDate.HasValue && model.ReminderOffsetMinutes.HasValue)
            {
                model.ReminderDate = model.AssignedDate.Value.AddMinutes(model.ReminderOffsetMinutes.Value);
            }

            var originalTask = await _taskRepository.GetTaskById(companyId, model.TaskId);
            if (originalTask == null) return false;

            // Safeguard structural IDs
            if (model.RecurringGroupId == null || model.RecurringGroupId == Guid.Empty)
            {
                model.RecurringGroupId = originalTask.RecurringGroupId;
            }
            if (model.RecurrenceParentId == null || model.RecurrenceParentId == 0)
            {
                model.RecurrenceParentId = originalTask.RecurrenceParentId;
            }

            if (updateType == "this" || originalTask.RecurringGroupId == null)
            {
                if ((originalTask.TaskStatus == "Completed" || originalTask.TaskStatus == "Canceled") && model.TaskStatus == originalTask.TaskStatus) 
                {
                    return false;
                }
                return await _taskRepository.UpdateTask(companyId, model);
            }

            if (updateType == "future" || updateType == "all")
            {
                var allGroupTasks = await _taskRepository.GetTasksByGroupId(companyId, originalTask.RecurringGroupId.Value);

                var tasksToUpdate = allGroupTasks.Where(t => t.TaskStatus != "Completed" && t.TaskStatus != "Canceled").ToList();
                
                if (updateType == "future" && originalTask.AssignedDate.HasValue)
                {
                    tasksToUpdate = tasksToUpdate.Where(t => t.AssignedDate >= originalTask.AssignedDate).ToList();
                }
                // If updateType == "all", we don't filter by date. We want to update all pending tasks in the series.

                var currentTaskInDb = tasksToUpdate.FirstOrDefault(t => t.TaskId == model.TaskId);
                var futureTasksToUpdate = tasksToUpdate.Where(t => t.TaskId != model.TaskId).OrderBy(t => t.AssignedDate).ToList();

                // Temporarily detach parent ID to force regeneration as root
                var trueParentId = model.RecurrenceParentId;
                model.RecurrenceParentId = null;
                var generatedOccurrences = _recurrenceService.GenerateOccurrences(model);
                model.RecurrenceParentId = trueParentId;

                int maxIterations = Math.Max(futureTasksToUpdate.Count, generatedOccurrences.Count);

                for (int i = 0; i < maxIterations; i++)
                {
                    if (i < futureTasksToUpdate.Count && i < generatedOccurrences.Count)
                    {
                        var t = futureTasksToUpdate[i];
                        var newOcc = generatedOccurrences[i];

                        t.Title = model.Title;
                        t.Description = model.Description;
                        t.AssignedUserId = model.AssignedUserId;
                        t.Priority = model.Priority;
                        t.TaskCategoryId = model.TaskCategoryId;
                        t.TaskStatus = model.TaskStatus;
                        
                        t.RecurrenceType = model.RecurrenceType;
                        t.RecurrenceInterval = model.RecurrenceInterval;
                        t.RecurrenceDays = model.RecurrenceDays;
                        t.RecurrenceRule = model.RecurrenceRule;
                        t.RecurrenceEndDate = model.RecurrenceEndDate;
                        t.RecurrenceOccurrenceCount = model.RecurrenceOccurrenceCount;

                        t.AssignedDate = newOcc.AssignedDate;
                        t.ExpectedDate = newOcc.ExpectedDate;

                        t.Reminder = model.Reminder;
                        t.ReminderOffsetMinutes = model.ReminderOffsetMinutes;
                        if (t.Reminder && t.AssignedDate.HasValue && t.ReminderOffsetMinutes.HasValue)
                        {
                            t.ReminderDate = t.AssignedDate.Value.AddMinutes(t.ReminderOffsetMinutes.Value);
                        }
                        else
                        {
                            t.ReminderDate = null;
                        }

                        await _taskRepository.UpdateTask(companyId, t);
                    }
                    else if (i >= futureTasksToUpdate.Count && i < generatedOccurrences.Count)
                    {
                        var newOcc = generatedOccurrences[i];
                        newOcc.RecurrenceParentId = trueParentId ?? model.TaskId;
                        await _taskRepository.AddTask(companyId, newOcc);
                    }
                    else if (i < futureTasksToUpdate.Count && i >= generatedOccurrences.Count)
                    {
                        await _taskRepository.DeleteTask(companyId, futureTasksToUpdate[i].TaskId);
                    }
                }

                if (currentTaskInDb != null)
                {
                    currentTaskInDb.Title = model.Title;
                    currentTaskInDb.Description = model.Description;
                    currentTaskInDb.AssignedUserId = model.AssignedUserId;
                    currentTaskInDb.Priority = model.Priority;
                    currentTaskInDb.Reminder = model.Reminder;
                    currentTaskInDb.ReminderOffsetMinutes = model.ReminderOffsetMinutes;
                    if (currentTaskInDb.Reminder && currentTaskInDb.AssignedDate.HasValue && currentTaskInDb.ReminderOffsetMinutes.HasValue)
                    {
                        currentTaskInDb.ReminderDate = currentTaskInDb.AssignedDate.Value.AddMinutes(currentTaskInDb.ReminderOffsetMinutes.Value);
                    }
                    else
                    {
                        currentTaskInDb.ReminderDate = null;
                    }

                    currentTaskInDb.TaskCategoryId = model.TaskCategoryId;
                    currentTaskInDb.TaskStatus = model.TaskStatus;
                    currentTaskInDb.RecurrenceType = model.RecurrenceType;
                    currentTaskInDb.RecurrenceInterval = model.RecurrenceInterval;
                    currentTaskInDb.RecurrenceDays = model.RecurrenceDays;
                    currentTaskInDb.RecurrenceRule = model.RecurrenceRule;
                    currentTaskInDb.RecurrenceEndDate = model.RecurrenceEndDate;
                    currentTaskInDb.RecurrenceOccurrenceCount = model.RecurrenceOccurrenceCount;
                    
                    currentTaskInDb.AssignedDate = model.AssignedDate;
                    currentTaskInDb.ExpectedDate = model.ExpectedDate;

                    await _taskRepository.UpdateTask(companyId, currentTaskInDb);
                }

                return true;
            }

            if ((originalTask.TaskStatus == "Completed" || originalTask.TaskStatus == "Canceled") && model.TaskStatus == originalTask.TaskStatus) return false;
            return await _taskRepository.UpdateTask(companyId, model);
        }

    }
}
