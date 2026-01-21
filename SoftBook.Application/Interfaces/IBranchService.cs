using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IBranchService
    {
        Task<List<Branch>> GetAllAsync();
        Task<Branch?> GetByIdAsync(int id);
        Task InsertAsync(Branch entity);
        Task UpdateAsync(Branch entity);
        Task DeleteAsync(int id);
    }
}
