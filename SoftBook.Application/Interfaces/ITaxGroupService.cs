using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ITaxGroupService
    {
        Task<List<TaxGroup>> GetAllAsync();
        Task<TaxGroup?> GetByIdAsync(int id);
        Task InsertAsync(TaxGroup entity);
        Task UpdateAsync(TaxGroup entity);
        Task DeleteAsync(int id);

        Task<List<TaxGroupWithRate>> GetBySupplyTypeWithRatesAsync(string supplyType);

  }
}
