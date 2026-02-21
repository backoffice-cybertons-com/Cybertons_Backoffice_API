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
    Task<DbResult> InsertAsync(Party party);
    Task<DbResult> UpdateAsync(Party party);
    Task<DbResult> DeleteAsync(int id);
    Task<List<Party>> GetByPartyTypeAsync(string partyType);
  }
}
// Customer, Vendor, Trader
