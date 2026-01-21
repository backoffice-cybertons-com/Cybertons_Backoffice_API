using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ITaxService
    {
        Task<List<Tax>> GetAllAsync();
        Task<Tax?> GetByIdAsync(int id);
        Task InsertAsync(Tax entity);
        Task UpdateAsync(Tax entity);
        Task DeleteAsync(int id);
    }
}
