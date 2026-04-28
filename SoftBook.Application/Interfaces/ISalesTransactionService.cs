using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ISalesTransactionService
    {
        Task<List<SalePur>> GetAllAsync(int companyId);
        Task<SalesTransactionViewModel?> GetByIdAsync(int companyId, int spUniqueCode);
        Task<DbResult> SaveCompositeAsync(SalesTransactionViewModel viewModel, int? userId = null);
        Task<DbResult> DeleteAsync(int companyId, int spUniqueCode, int? userId = null);
        Task<decimal> GetPartyOutstandingAsync(int companyId, int partyId);
    }
}
