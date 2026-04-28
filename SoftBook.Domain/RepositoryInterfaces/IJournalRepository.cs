using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IJournalRepository
    {
        Task<DbResult> InsertAsync(JournalVoucher entity, IDbTransaction? transaction = null, int? userId = null);
        Task<DbResult> DeleteAsync(long jvCode);
        Task<List<JournalVoucher>> GetByVoucherAsync(string vType, long vCode);
        Task<DbResult> SaveJournalGroupAsync(IEnumerable<JournalVoucher> entries, string? gCodeToDelete = null, IDbTransaction? transaction = null, int? userId = null);
        Task<List<JournalVoucher>> GetByGCodeAsync(string gCode);
        Task<IEnumerable<JournalVoucher>> GetDirectJournalsAsync(long companyId, string vType);
    }
}
