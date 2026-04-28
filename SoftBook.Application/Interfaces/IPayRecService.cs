using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPayRecService
    {
        Task<IEnumerable<dynamic>> GetPaymentListAsync(long companyId, int pageNo, int pageSize, string? vType, int? partyId);
        Task<IEnumerable<dynamic>> GetPendingInvoicesAsync(string trnType, int partyId);
        Task<List<PayRec>> GetByIdAsync(long companyId, long code);
        Task<DbResult> SaveCompositeAsync(IEnumerable<PayRec> entities, int? userId = null);
        Task<DbResult> DeleteAsync(long companyId, long code, string deletedBy, int? userId = null);
    }
}
