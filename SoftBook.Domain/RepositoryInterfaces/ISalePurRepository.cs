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
		Task<DbResult> InsertAsync(SalePur entity, int? userId = null);
		Task<DbResult> UpdateAsync(SalePur entity, int? userId = null);
		Task<DbResult> DeleteAsync(int companyId, int spUniqueCode, int? userId = null);
		Task<decimal> GetPartyOutstandingAsync(int companyId, int partyId);
		Task<decimal> GetPartyOutstandingBeforeDateAsync(int companyId, int partyId, DateTime beforeDate);
		Task<decimal> GetTodaysReceiptAsync(int companyId, int partyId, DateTime date);

		Task<List<SalePurTran>> GetLineItemsAsync(int spUniqueCode);
		Task<DbResult> InsertLineItemAsync(SalePurTran entity);
		Task<DbResult> UpdateLineItemAsync(SalePurTran entity);
		Task<DbResult> DeleteLineItemAsync(int spTranUniqueCode);
	}
}
