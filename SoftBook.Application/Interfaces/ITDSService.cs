using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ITDSService
    {
        Task<List<TDS>> GetAllAsync();
        Task<TDS?> GetByIdAsync(int id);
        Task InsertAsync(TDS entity);
        Task UpdateAsync(TDS entity);
        Task DeleteAsync(int id);
    }
}
