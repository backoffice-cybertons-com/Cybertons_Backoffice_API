using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPartyAddressService
    {
        Task<List<PartyAddress>> GetAllAsync();
        Task<PartyAddress?> GetByIdAsync(int id);
        Task<List<PartyAddress>> GetByPartyIdAsync(int partyId);
        Task InsertAsync(PartyAddress entity);
        Task UpdateAsync(PartyAddress entity);
        Task DeleteAsync(int id);
    }
}







