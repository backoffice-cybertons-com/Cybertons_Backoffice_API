using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IMobileOrderSyncService
    {
        Task<DbResult> SyncPendingOrdersAsync(int companyId, int? userId = null);
    }
}
