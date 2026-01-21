using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<List<Currency>> GetAllAsync();
        Task<Currency?> GetByIdAsync(int id);
        Task InsertAsync(Currency entity);
        Task UpdateAsync(Currency entity);
        Task DeleteAsync(int id);
    }
}
