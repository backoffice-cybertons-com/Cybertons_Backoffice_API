using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPartyService
    {
        Task<List<Party>> GetAllAsync();
        Task<Party?> GetByIdAsync(int id);
        Task<List<Party>> GetByPartyTypeAsync(string partyType);
        Task<DbResult> InsertAsync(Party entity);
        Task<DbResult> UpdateAsync(Party entity);
        Task<DbResult> DeleteAsync(int id);
        Task<DbResult> InsertCompositeAsync(PartyViewModel viewModel);
        Task<DbResult> UpdateCompositeAsync(PartyViewModel viewModel);
        Task<PartyViewModel?> GetCompositeAsync(int id);
    }
}
