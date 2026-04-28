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
        Task<LedgerReportDTO> GetLedgerReportAsync(int accountCode, DateTime fromDate, DateTime toDate);

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
    }
}
