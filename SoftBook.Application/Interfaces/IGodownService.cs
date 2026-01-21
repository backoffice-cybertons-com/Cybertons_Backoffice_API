using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IGodownService
    {
        Task<List<Godown>> GetAllAsync();
        Task<Godown?> GetByIdAsync(int id);
        Task InsertAsync(Godown entity);
        Task UpdateAsync(Godown entity);
        Task DeleteAsync(int id);
    }
}
