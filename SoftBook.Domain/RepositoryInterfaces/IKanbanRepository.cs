using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IKanbanRepository
    {
        Task EnsureSchemaAndDefaultsAsync(int companyId);
        Task<List<KanbanColumnModel>> GetColumnsAsync(int companyId);
        Task<bool> SaveColumnAsync(int companyId, KanbanColumnModel column);
        Task<bool> SaveTaskAsync(int companyId, KanbanTaskModel task);
        Task<bool> DeleteColumnAsync(int companyId, string columnId);
        Task<bool> DeleteTaskAsync(int companyId, string taskId);
        Task<List<KanbanChecklistItemModel>> GetChecklistItemsAsync(long taskId);
        Task<KanbanChecklistItemModel> SaveChecklistItemAsync(KanbanChecklistItemModel item);
        Task<bool> DeleteChecklistItemAsync(long itemId);
        Task<bool> DeleteChecklistGroupAsync(long taskId, string groupName);
    }
}
