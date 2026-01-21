using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
  public interface ITaxGroupRepository : IRepository<TaxGroup>
  {
    Task<List<TaxGroupWithRate>> GetBySupplyTypeWithRatesAsync(string supplyType);
  }
}
