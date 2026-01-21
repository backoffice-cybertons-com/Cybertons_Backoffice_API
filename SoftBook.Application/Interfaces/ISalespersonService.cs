using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ISalespersonService
    {
        Task<List<Salesperson>> GetAllAsync();
        Task<Salesperson?> GetByIdAsync(int id);
        Task InsertAsync(Salesperson entity);
        Task UpdateAsync(Salesperson entity);
        Task DeleteAsync(int id);
    }
}
