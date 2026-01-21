using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<List<AccountHead>> GetAllAsync();
        Task<AccountHead?> GetByIdAsync(int id);
        Task InsertAsync(AccountHead entity);
        Task UpdateAsync(AccountHead entity);
        Task DeleteAsync(int id);
    }
}
