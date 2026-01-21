using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IPartyBankDetailsRepository : IRepository<PartyBankDetails>
    {
        Task<List<PartyBankDetails>> GetByPartyIdAsync(int partyId);
    }
}







