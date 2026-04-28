//using System;
//using System.Threading.Tasks;
//using SoftBook.Application.Services.Interfaces;
//using SoftBook.Domain.DTOs;
//using SoftBook.Domain.RepositoryInterfaces;

//namespace SoftBook.Application.Services.Classes
//{
//    public class ReportService : IReportService
//    {
//        private readonly IReportRepository _reportRepository;

//        public ReportService(IReportRepository reportRepository)
//        {
//            _reportRepository = reportRepository;
//        }

//        public async Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate)
//        {
//            return await _reportRepository.GetLedgerReportAsync(accountCode, fromDate, toDate);
//        }

//        public async Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType)
//        {
//            return await _reportRepository.GetSalePurchaseRegisterItemwiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//        }
//        public async Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType)
//        {
//            return await _reportRepository.GetSalePurchaseRegisterDatewiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//        }
//        public async Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType)
//        {
//            return await _reportRepository.GetSalePurchaseRegisterPartywiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//        }

//        public async Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, DateTime? fromDate, DateTime? toDate, string? productName, string? customerName, string? invoiceNo, string? invType)
//        {
//            return await _reportRepository.GetSalePurInvWiseAsync(companyId, fromDate, toDate, productName, customerName, invoiceNo, invType);
//        }
//    }
//}


using System;
using System.Threading.Tasks;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Application.Services.Classes
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate)
        {
            return await _reportRepository.GetLedgerReportAsync(accountCode, fromDate, toDate);
        }

        public async Task<IEnumerable<SalePurchaseRegisterItemwiseDTO>> GetSalePurchaseRegisterItemwiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurchaseRegisterItemwiseAsync(companyId, invType, where);
        }
        public async Task<IEnumerable<SalePurchaseRegisterDatewiseDTO>> GetSalePurchaseRegisterDatewiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurchaseRegisterDatewiseAsync(companyId, invType, where);
        }
        public async Task<IEnumerable<SalePurchaseRegisterPartywiseDTO>> GetSalePurchaseRegisterPartywiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurchaseRegisterPartywiseAsync(companyId, invType, where);
        }
        public async Task<List<InvWiseDTO>> GetSalePurInvWiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurInvWiseAsync(companyId, invType, where);
        }

        public async Task<IEnumerable<SalePurchaseRegisterMonthwiseDTO>> GetSalePurchaseRegisterMonthwiseAsync(int companyId, string? invType, string? where)
        {
            return await _reportRepository.GetSalePurchaseRegisterMonthwiseAsync(companyId, invType, where);
        }

        //----------------------trial balance code - 230426 hetak modi--------------------------------------------------------------------------------------------------
        public async Task<List<TrialBalanceDTO>> GetTrialBalance(int companyId, DateTime date)
        {
            return await _reportRepository.GetTrialBalance(companyId, date);
        }



        //by diya 4/24
        //payeble receivable
        public async Task<IEnumerable<PayebleReceivableDTO>> GetPayebleReceivableAsync(int companyId, int? partyId, string? pAsOfDate, string partyType)
        {
            return await _reportRepository.GetPayebleReceivableAsync(companyId, partyId, pAsOfDate, partyType);
        }

        public async Task<IEnumerable<SalePurchaseRegisterDatewiseComparisonDTO>> GetSalePurchaseRegisterDatewiseComparisonAsync(int companyId, string? where, string compareMode)
        {
            return await _reportRepository.GetSalePurchaseRegisterDatewiseComparisonAsync(companyId, where, compareMode);
        }


        //by diya 4/27
        //GSTR-3b
        public async Task<IEnumerable<GetGSTR3bDTO>> GetGSTR3bAsync(int companyId, string? where, string Type)
        {
            return await _reportRepository.GetGSTR3bAsync(companyId, where, Type);
        }
    }
}

