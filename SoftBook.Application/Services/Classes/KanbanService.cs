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

        public async Task<List<KanbanBoardModel>> GetBoardsAsync(int companyId)
        {
            return await _kanbanRepository.GetBoardsAsync(companyId);
        }

        public async Task<KanbanBoardModel> CreateBoardAsync(int companyId, string boardName)
        {
            return await _kanbanRepository.CreateBoardAsync(companyId, boardName);
        }

        public async Task<List<KanbanColumnModel>> GetColumnsAsync(int companyId, long boardId)
        {
            return await _kanbanRepository.GetColumnsAsync(companyId, boardId);
        }

        public async Task<bool> SaveColumnAsync(int companyId, long boardId, KanbanColumnModel column)
        {
            return await _kanbanRepository.SaveColumnAsync(companyId, boardId, column);
        }

        public async Task<bool> SaveTaskAsync(int companyId, long boardId, KanbanTaskModel task)
        {
            return await _kanbanRepository.SaveTaskAsync(companyId, boardId, task);
        }

        public async Task<bool> DeleteColumnAsync(int companyId, long boardId, string columnId)
        {
            return await _kanbanRepository.DeleteColumnAsync(companyId, boardId, columnId);
        }

        public async Task<bool> DeleteTaskAsync(int companyId, long boardId, string taskId)
        {
            return await _kanbanRepository.DeleteTaskAsync(companyId, boardId, taskId);
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
