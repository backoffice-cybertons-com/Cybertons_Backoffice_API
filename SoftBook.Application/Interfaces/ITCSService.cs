using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ITCSService
    {
        Task<List<TCS>> GetAllAsync();
        Task<TCS?> GetByIdAsync(int id);
        Task InsertAsync(TCS entity);
        Task UpdateAsync(TCS entity);
        Task DeleteAsync(int id);
    }
}
