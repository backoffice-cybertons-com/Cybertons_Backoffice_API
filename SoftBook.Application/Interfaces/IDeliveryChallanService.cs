using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IDeliveryChallanService
    {
        Task<List<DeliveryChallan>> GetAllAsync(int companyId);
        Task<List<dynamic>> GetAllDetailedAsync(int companyId);
        Task<DeliveryChallanViewModel?> GetByIdAsync(int companyId, int deliveryChallanId);
        Task<DbResult> InsertCompositeAsync(DeliveryChallanViewModel viewModel);
        Task<DbResult> UpdateCompositeAsync(DeliveryChallanViewModel viewModel);
        Task<DbResult> DeleteAsync(int companyId, int deliveryChallanId);
    }
}
