//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using SoftBook.Domain.DTOs;

//namespace SoftBook.Domain.RepositoryInterfaces
//{
//    public interface IReportRepository
//    {
//        Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate);

//        Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo,
//                                        string? invType);

//        Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType);
//        Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType);
//        Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType);



//    }
//}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IReportRepository
    {
        Task<LedgerReportDTO> GetLedgerReportAsync(int companyId, int accountCode, DateTime fromDate, DateTime toDate);

        Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, string? invType, string? where);

        Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, string? invType, string? where);
        Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, string? invType, string? where);
        Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, string? invType, string? where);

        Task<IEnumerable<SalePurchaseRegisterMonthwiseDTO>> GetSalePurchaseRegisterMonthwiseAsync(int companyId, string? invType, string? where);

        //Trial balance  code added by Hetak Modi23042026
        //=======================================================================================================================================
        Task<List<TrialBalanceDTO>> GetTrialBalance(int companyId, DateTime date);



        Task<IEnumerable<PayebleReceivableDTO>> GetPayebleReceivableAsync(int companyId, int? partyId, string pAsOfDate, string partyType);

        Task<IEnumerable<SalePurchaseRegisterDatewiseComparisonDTO>> GetSalePurchaseRegisterDatewiseComparisonAsync(int companyId, string? where, string compareMode);
        //gst
        Task<IEnumerable<GetGSTR3bDTO>> GetGSTR3bAsync(int companyId, string? where, string Type);

        // Ledger Summary Datewise - Hetak Modi 02-05-2026
        Task<IEnumerable<LedgerSummaryDatewiseDTO>> GetLedgerSummaryDatewiseAsync(int companyId, int accountCode, DateTime fromDate, DateTime toDate);
     //   Task<IEnumerable<GetGSTR3bDTO>> GetGSTR3bAsync(int companyId, string where, string Type);
        Task<List<GstrExcelRow>> GetGstr1ExcelExportAsync(
   int companyId,
   DateOnly fromDate,
   DateOnly toDate);

        Task<IEnumerable<OutstandingComparisonDTO>> GetOutstandingComparisonAsync(int companyId, DateOnly Date,string? where,string mode);

        // Created by Hetak on 08052026
        Task<List<StockSummaryReportDTO>> GetStockSummaryReportAsync(int companyId, DateTime fromDate, DateTime toDate);
    }
}
