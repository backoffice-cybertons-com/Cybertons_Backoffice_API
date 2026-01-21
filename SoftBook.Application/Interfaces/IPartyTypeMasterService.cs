using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPartyTypeMasterService
    {
        Task<List<PartyTypeMaster>> GetAllAsync();
        Task<PartyTypeMaster?> GetByIdAsync(int id);
        Task<List<Party>> GetPartiesByTypeAsync(string partyType);
        Task InsertAsync(PartyTypeMaster entity);
        Task UpdateAsync(PartyTypeMaster entity);
        Task DeleteAsync(int id);
    }
}




