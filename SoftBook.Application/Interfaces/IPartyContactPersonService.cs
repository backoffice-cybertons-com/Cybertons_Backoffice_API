using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPartyContactPersonService
    {
        Task<List<PartyContactPerson>> GetAllAsync();
        Task<PartyContactPerson?> GetByIdAsync(int id);
        Task<List<PartyContactPerson>> GetByPartyIdAsync(int partyId);
        Task InsertAsync(PartyContactPerson entity);
        Task UpdateAsync(PartyContactPerson entity);
        Task DeleteAsync(int id);
    }
}







