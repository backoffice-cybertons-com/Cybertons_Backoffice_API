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
using System.Globalization;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<LedgerReportDTO> GetLedgerReportAsync(int companyId, int accountCode, DateTime fromDate, DateTime toDate);

        Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, string? invType, string? where);


        Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, string? invType, string? where);
        Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, string? invType, string? where);
        Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, string? invType, string? where);

        Task<IEnumerable<SalePurchaseRegisterMonthwiseDTO>> GetSalePurchaseRegisterMonthwiseAsync(int companyId, string? invType, string? where);
        //==============================================trial balance hetak modi230426======================================================================
        Task<List<TrialBalanceDTO>> GetTrialBalance(int companyId, DateTime date);
        Task<List<BalanceSheetReportDTO>> GetBalanceSheetReportAsync(int companyId, DateTime asOfDate);
        Task<List<BalanceSheetReportDTO>> GetProfitLossReportAsync(int companyId, DateTime asOfDate);

        Task<IEnumerable<PayebleReceivableDTO>> GetPayebleReceivableAsync(int companyId, int? partyId, string? pAsOfDate, string partyType);


        Task<IEnumerable<SalePurchaseRegisterDatewiseComparisonDTO>> GetSalePurchaseRegisterDatewiseComparisonAsync(int companyId, string? where, string compareMode);


        //by Diya 27/04
        Task<IEnumerable<GetGSTR3bDTO>> GetGSTR3bAsync(int companyId, string? where, string Type);

        // Ledger Summary Datewise - Hetak Modi 02-05-2026
        Task<IEnumerable<LedgerSummaryDatewiseDTO>> GetLedgerSummaryDatewiseAsync(int companyId, int accountCode, DateTime fromDate, DateTime toDate);


        Task<byte[]> GenerateGstr1ExcelAsync( int companyId, DateOnly fromDate, DateOnly toDate);

        //by Diya 05/04
        Task<IEnumerable<OutstandingComparisonDTO>> GetOutstandingComparisonAsync(int companyId, DateOnly Date ,string? where,string mode);

        // Created by Hetak on 08052026
        Task<List<StockSummaryReportDTO>> GetStockSummaryReportAsync(int companyId, DateTime fromDate, DateTime toDate);

        // Developed by Hetak on 11-05-2026
        Task<IEnumerable<StockLedgerDatewiseDTO>> GetStockLedgerDatewiseAsync(int companyId, DateTime fromDate, DateTime toDate);

        Task<IEnumerable<SalePurchaseRegisterTransactionwiseDTO>> GetSalePurchaseRegisterTransactionWise(int companyId, string? invType, string? where);

        Task<IEnumerable<DailyOutstandingDto>> GetDailyOutstandingReportAsync(int companyId, string fromDate, string toDate, int? partyId);

        Task<IEnumerable<CalendarSummaryDto>> GetCalendarSummaryReportAsync(long companyId, DateTime fromDate, DateTime toDate,int? partyId);
    }
}
