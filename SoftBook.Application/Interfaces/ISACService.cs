using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ISACService
    {
        Task<List<SAC>> GetAllAsync();
        Task<SAC?> GetByIdAsync(int id);
        Task InsertAsync(SAC entity);
        Task UpdateAsync(SAC entity);
        Task DeleteAsync(int id);
    }
}
