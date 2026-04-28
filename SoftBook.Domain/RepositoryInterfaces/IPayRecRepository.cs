using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IPayRecRepository
    {
        Task<List<PayRec>> GetAllAsync(long companyId, string? vchType = null);
        Task<List<PayRec>> GetByIdAsync(long companyId, long code);
        Task<DbResult> InsertAsync(PayRec entity, IDbTransaction? transaction = null, int? userId = null);
        Task<DbResult> UpdateAsync(PayRec entity, int? userId = null);
        Task<DbResult> DeleteAsync(long companyId, long code, string deletedBy, int? userId = null);
        Task<DbResult> DeleteByGCodeAsync(string gCode, string deletedBy, IDbTransaction? transaction = null, int? userId = null);
        
        // Custom methods for functions
        Task<IEnumerable<dynamic>> GetPendingInvoicesAsync(string trnType, int partyId);
        Task<IEnumerable<dynamic>> GetPaymentListAsync(long companyId, int pageNo, int pageSize, string? vType, int? partyId);
        string GetConnectionStringForService();
    }
}
