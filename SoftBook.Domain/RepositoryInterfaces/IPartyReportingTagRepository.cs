using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IPartyReportingTagRepository : IRepository<PartyReportingTag>
    {
        Task<List<PartyReportingTag>> GetByPartyIdAsync(int partyId);
    }
}







