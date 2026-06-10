using System;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Domain.RepositoryInterfaces
{
  public interface IPartyRepository
  {
    Task<List<Party>> GetAllAsync();
    Task<Party?> GetByIdAsync(int id);
    Task<DbResult> InsertAsync(Party party, int? userId = null);
    Task<DbResult> UpdateAsync(Party party, int? userId = null);
    Task<DbResult> DeleteAsync(int id, int? userId = null);
    Task<List<Party>> GetByPartyTypeAsync(string partyType);
    Task<bool> UpdateRateDiffAsync(int companyId, int partyId, decimal rateDiff);
    Task<Dictionary<string, PaymentTermLookupDto>> GetPaymentTermLookupAsync(int companyId);
  }
}
// Customer, Vendor, Trader
