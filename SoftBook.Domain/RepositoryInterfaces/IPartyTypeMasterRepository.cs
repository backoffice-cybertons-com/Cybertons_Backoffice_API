using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IPartyTypeMasterRepository : IRepository<PartyTypeMaster>
    {
        Task<List<Party>> GetPartiesByTypeAsync(string partyType); // Customer, Vendor, Trader
    }
}




