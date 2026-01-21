using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IBankService
    {
        Task<List<Bank>> GetAllAsync();
        Task<Bank?> GetByIdAsync(int id);
        Task InsertAsync(Bank entity);
        Task UpdateAsync(Bank entity);
        Task DeleteAsync(int id);
    }
}
