using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task InsertAsync(Product entity);
        Task UpdateAsync(Product entity);
        Task DeleteAsync(int id);

    Task<List<ProductWithDetails>> GetAllWithDetailsAsync();

  }
}
