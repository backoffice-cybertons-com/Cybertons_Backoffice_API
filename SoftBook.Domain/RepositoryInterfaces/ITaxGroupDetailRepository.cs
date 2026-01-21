using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface ITaxGroupDetailRepository
    {
        Task<List<TaxGroupDetail>> GetAllAsync();
        Task<TaxGroupDetail?> GetByIdAsync(int id);
        Task<List<TaxGroupDetail>> GetByTaxGroupIdAsync(int taxGroupId);
        Task InsertAsync(TaxGroupDetail entity);
        Task UpdateAsync(TaxGroupDetail entity);
        Task DeleteAsync(int id);
    }
}


