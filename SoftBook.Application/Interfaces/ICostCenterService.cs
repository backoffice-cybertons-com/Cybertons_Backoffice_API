using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ICostCenterService
    {
        Task<List<CostCenter>> GetAllAsync();
        Task<CostCenter?> GetByIdAsync(int id);
        Task InsertAsync(CostCenter entity);
        Task UpdateAsync(CostCenter entity);
        Task DeleteAsync(int id);
    }
}
