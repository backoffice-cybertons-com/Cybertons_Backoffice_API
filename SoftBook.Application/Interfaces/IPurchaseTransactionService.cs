using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPurchaseTransactionService
    {
        Task<List<SalePur>> GetAllAsync(int companyId);
        Task<PurchaseTransactionViewModel?> GetByIdAsync(int companyId, int spUniqueCode);
        Task<DbResult> SaveCompositeAsync(PurchaseTransactionViewModel viewModel);
        Task<DbResult> DeleteAsync(int companyId, int spUniqueCode);
    }
}
