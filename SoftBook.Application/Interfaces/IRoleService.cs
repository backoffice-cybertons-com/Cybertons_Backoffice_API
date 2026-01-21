using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task InsertAsync(Role entity);
        Task UpdateAsync(Role entity);
        Task DeleteAsync(int id);
    }
}
