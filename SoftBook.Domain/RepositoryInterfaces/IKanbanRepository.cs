using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IKanbanRepository
    {
        Task EnsureSchemaAndDefaultsAsync(int companyId);
        Task<List<KanbanBoardModel>> GetBoardsAsync(int companyId);
        Task<KanbanBoardModel> CreateBoardAsync(int companyId, string boardName);
        Task<List<KanbanColumnModel>> GetColumnsAsync(int companyId, long boardId);
        Task<bool> SaveColumnAsync(int companyId, long boardId, KanbanColumnModel column);
        Task<bool> SaveTaskAsync(int companyId, long boardId, KanbanTaskModel task);
        Task<bool> DeleteColumnAsync(int companyId, long boardId, string columnId);
        Task<bool> DeleteTaskAsync(int companyId, long boardId, string taskId);
        Task<List<KanbanChecklistItemModel>> GetChecklistItemsAsync(long taskId);
        Task<KanbanChecklistItemModel> SaveChecklistItemAsync(KanbanChecklistItemModel item);
        Task<bool> DeleteChecklistItemAsync(long itemId);
        Task<bool> DeleteChecklistGroupAsync(long taskId, string groupName);
    }
}
