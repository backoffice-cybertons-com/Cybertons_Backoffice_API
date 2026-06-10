//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Data;
//using SoftBook.Domain.Entities;
//using SoftBook.Domain.DTOs;

//namespace SoftBook.Domain.RepositoryInterfaces
//{
//    public interface ISalePurRepository
//    {
//        Task<List<SalePur>> GetAllAsync(int companyId);
//        Task<SalePur?> GetByIdAsync(int companyId, int spUniqueCode);
//        Task<DbResult> InsertAsync(SalePur entity);
//        Task<DbResult> UpdateAsync(SalePur entity);
//        Task<DbResult> DeleteAsync(int companyId, int spUniqueCode);
//        Task<decimal> GetPartyOutstandingAsync(int companyId, int partyId);

//        Task<List<SalePurTran>> GetLineItemsAsync(int spUniqueCode);
//        Task<DbResult> InsertLineItemAsync(SalePurTran entity);
//        Task<DbResult> UpdateLineItemAsync(SalePurTran entity);
//        Task<DbResult> DeleteLineItemAsync(int spTranUniqueCode);
//    }
//}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
	public interface ISalePurRepository
	{
		Task<List<SalePur>> GetAllAsync(int companyId);
		Task<SalePur?> GetByIdAsync(int companyId, int spUniqueCode);
		// changes made by Hetak dt 20052026 - allow sale/purchase save steps to share one DB transaction.
		Task<DbResult> ExecuteInTransactionAsync(Func<IDbTransaction, Task<DbResult>> operation);
		Task<DbResult> InsertAsync(SalePur entity, int? userId = null, IDbTransaction? transaction = null);
		Task<DbResult> InsertWithNextInvoiceNumberAsync(SalePur entity, int? userId = null, IDbTransaction? transaction = null);
		Task<DbResult> UpdateAsync(SalePur entity, int? userId = null, IDbTransaction? transaction = null);
		Task<DbResult> DeleteAsync(int companyId, int spUniqueCode, int? userId = null);
		Task<decimal> GetPartyOutstandingAsync(int companyId, int partyId);
		Task<decimal> GetPartyOutstandingBeforeDateAsync(int companyId, int partyId, DateTime beforeDate);
		Task<decimal> GetTodaysReceiptAsync(int companyId, int partyId, DateTime date);
		Task<decimal> GetTodaysInvoiceAmountAsync(int companyId, int partyId, DateTime date);
		Task<decimal> GetPartyLast7DaysSalesTotalAsync(int companyId, int partyId);
		Task<decimal> GetPartyLast7DaysSalesTotalAsOfDateAsync(int companyId, int partyId, DateTime asOfDate);
		Task<List<SalesInvoiceAddPriceCheckDto>> GetSalesInvoicesForAddPriceCheckAsync(int companyId, DateTime fromDate, DateTime toDate, string? partyType = null);
		Task<decimal> GetAddPriceForDueDaysAsync(int companyId, decimal avgOutstandingDays);
		Task<AddPriceLookupDto> GetAddPriceLookupForDueDaysAsync(int companyId, decimal avgOutstandingDays);

		Task<List<SalePurTran>> GetLineItemsAsync(int spUniqueCode);
		Task<DbResult> InsertLineItemAsync(SalePurTran entity, IDbTransaction? transaction = null);
		Task<DbResult> UpdateLineItemAsync(SalePurTran entity);
		Task<DbResult> DeleteLineItemAsync(int spTranUniqueCode);
		// [ADDED 2026-05-06] New method to soft-delete all lines for an invoice header
		Task<DbResult> DeleteLineItemsByParentAsync(int spUniqueCode, int? userId = null, IDbTransaction? transaction = null);
	}
}
