using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<List<Warehouse>> GetAllAsync();
        Task<Warehouse?> GetByIdAsync(int id);
        Task InsertAsync(Warehouse entity);
        Task UpdateAsync(Warehouse entity);
        Task DeleteAsync(int id);
    }
}
