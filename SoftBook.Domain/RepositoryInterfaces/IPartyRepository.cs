using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IPartyRepository : IRepository<Party>
    {
        Task<List<Party>> GetAllAsync();
        Task<Party?> GetByIdAsync(int id);
        Task InsertAsync(Party entity);
        Task UpdateAsync(Party entity);
        Task<List<Party>> GetByPartyTypeAsync(string partyType); // Customer, Vendor, Trader
    }
}
