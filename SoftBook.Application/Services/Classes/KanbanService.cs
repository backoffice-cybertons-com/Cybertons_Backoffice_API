using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Classes
{
    public class KanbanService : IKanbanService
    {
        private readonly IKanbanRepository _kanbanRepository;

        public KanbanService(IKanbanRepository kanbanRepository)
        {
            _kanbanRepository = kanbanRepository;
        }

        public async Task<List<KanbanColumnModel>> GetColumnsAsync(int companyId)
        {
            return await _kanbanRepository.GetColumnsAsync(companyId);
        }

        public async Task<bool> SaveColumnAsync(int companyId, KanbanColumnModel column)
        {
            return await _kanbanRepository.SaveColumnAsync(companyId, column);
        }

        public async Task<bool> SaveTaskAsync(int companyId, KanbanTaskModel task)
        {
            return await _kanbanRepository.SaveTaskAsync(companyId, task);
        }

        public async Task<bool> DeleteColumnAsync(int companyId, string columnId)
        {
            return await _kanbanRepository.DeleteColumnAsync(companyId, columnId);
        }

        public async Task<bool> DeleteTaskAsync(int companyId, string taskId)
        {
            return await _kanbanRepository.DeleteTaskAsync(companyId, taskId);
        }

        public async Task<List<KanbanChecklistItemModel>> GetChecklistItemsAsync(long taskId)
        {
            return await _kanbanRepository.GetChecklistItemsAsync(taskId);
        }

        public async Task<KanbanChecklistItemModel> SaveChecklistItemAsync(KanbanChecklistItemModel item)
        {
            return await _kanbanRepository.SaveChecklistItemAsync(item);
        }

        public async Task<bool> DeleteChecklistItemAsync(long itemId)
        {
            return await _kanbanRepository.DeleteChecklistItemAsync(itemId);
        }

        public async Task<bool> DeleteChecklistGroupAsync(long taskId, string groupName)
        {
            return await _kanbanRepository.DeleteChecklistGroupAsync(taskId, groupName);
        }
    }
}
