using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IRecurrenceService
    {
        /// <summary>
        /// Generates a list of child tasks based on the parent task's recurrence settings.
        /// </summary>
        /// <param name="parentTask">The master recurring task.</param>
        /// <returns>List of generated occurrences.</returns>
        List<TaskModel> GenerateOccurrences(TaskModel parentTask);
    }
}
