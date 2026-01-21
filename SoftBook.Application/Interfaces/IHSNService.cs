using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IHSNService
    {
        Task<List<HSN>> GetAllAsync();
        Task<HSN?> GetByIdAsync(int id);
        Task InsertAsync(HSN entity);
        Task UpdateAsync(HSN entity);
        Task DeleteAsync(int id);
    }
}
