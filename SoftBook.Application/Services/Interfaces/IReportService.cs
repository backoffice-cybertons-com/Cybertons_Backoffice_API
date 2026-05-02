//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using SoftBook.Domain.DTOs;

//namespace SoftBook.Application.Services.Interfaces
//{
//    public interface IReportService
//    {
//        Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate);

//        Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType);


//        Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType);
//        Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType);
//        Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType);
//    }
//}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate);

        Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, string? invType, string? where);


        Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, string? invType, string? where);
        Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, string? invType, string? where);
        Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, string? invType, string? where);

        Task<IEnumerable<SalePurchaseRegisterMonthwiseDTO>> GetSalePurchaseRegisterMonthwiseAsync(int companyId, string? invType, string? where);
        //==============================================trial balance hetak modi230426======================================================================
        Task<List<TrialBalanceDTO>> GetTrialBalance(int companyId, DateTime date);

        Task<IEnumerable<PayebleReceivableDTO>> GetPayebleReceivableAsync(int companyId, int? partyId, string? pAsOfDate, string partyType);
        Task<IEnumerable<SalePurchaseRegisterDatewiseComparisonDTO>> GetSalePurchaseRegisterDatewiseComparisonAsync(int companyId, string? where, string compareMode);


        //by Diya 27/04
        Task<IEnumerable<GetGSTR3bDTO>> GetGSTR3bAsync(int companyId, string? where, string Type);

        // Ledger Summary Datewise - Hetak Modi 02-05-2026
        Task<IEnumerable<LedgerSummaryDatewiseDTO>> GetLedgerSummaryDatewiseAsync(int accountCode, DateTime fromDate, DateTime toDate);


        Task<byte[]> GenerateGstr1ExcelAsync(
 int companyId,
 DateOnly fromDate,
 DateOnly toDate);
    }
}
