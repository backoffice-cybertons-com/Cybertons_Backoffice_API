using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IJournalService
    {
        Task<DbResult> GenerateJournalFromSaleAsync(SalePur header, IEnumerable<SalePurTran> lines);
        Task<DbResult> GenerateJournalFromPurchaseAsync(SalePur header, IEnumerable<SalePurTran> lines);
        Task<DbResult> GenerateJournalFromPayRecAsync(IEnumerable<PayRec> allocations, IDbTransaction? transaction = null);
        Task<DbResult> DeleteJournalByGCodeAsync(string gCode, int? userId = null);
        Task<DbResult> SaveDirectJournalAsync(IEnumerable<JournalVoucher> entries, int? userId = null);
        Task<IEnumerable<JournalVoucher>> GetDirectJournalsAsync(long companyId, string vType);
        Task<object?> GetLedgerSettingsAsync(int? companyId = null);
    }
}
