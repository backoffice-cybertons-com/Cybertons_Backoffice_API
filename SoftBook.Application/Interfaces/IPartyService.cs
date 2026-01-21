using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPartyService
    {
        Task<List<Party>> GetAllAsync();
        Task<Party?> GetByIdAsync(int id);
        Task<List<Party>> GetByPartyTypeAsync(string partyType);
        Task InsertAsync(Party entity);
        Task UpdateAsync(Party entity);
        Task DeleteAsync(int id);
        Task InsertCompositeAsync(PartyViewModel viewModel);
        Task UpdateCompositeAsync(PartyViewModel viewModel);
        Task<PartyViewModel?> GetCompositeAsync(int id);
    }
}
