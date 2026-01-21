using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IUnitService
    {
        Task<List<Unit>> GetAllAsync();
        Task<Unit?> GetByIdAsync(int id);
        Task InsertAsync(Unit entity);
        Task UpdateAsync(Unit entity);
        Task DeleteAsync(int id);
    }
}
