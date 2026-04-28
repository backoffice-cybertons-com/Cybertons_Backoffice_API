using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IDeliveryChallanRepository
    {
        Task<List<DeliveryChallan>> GetAllAsync(int companyId);
        Task<DeliveryChallan?> GetByIdAsync(int companyId, int deliveryChallanId);
        Task<DbResult> InsertAsync(DeliveryChallan entity);
        Task<DbResult> UpdateAsync(DeliveryChallan entity);
        Task<DbResult> DeleteAsync(int companyId, int deliveryChallanId);

        Task<List<DeliveryChallanItem>> GetItemsAsync(int companyId, int deliveryChallanId);
        Task<DbResult> InsertItemAsync(DeliveryChallanItem entity);
        Task<DbResult> UpdateItemAsync(DeliveryChallanItem entity);
        Task<DbResult> DeleteItemAsync(int companyId, int itemId);
        
        Task<List<DeliveryChallanItem>> GetAllWithItemsAsync(int companyId, int? deliveryChallanId = null);
        Task<List<dynamic>> GetAllDetailedAsync(int companyId);
    }
}
