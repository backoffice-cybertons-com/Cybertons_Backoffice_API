using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class TaxGroupService : ITaxGroupService
    {
        private readonly ITaxGroupRepository _repository;

        public TaxGroupService(ITaxGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaxGroup>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TaxGroup?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(TaxGroup entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(TaxGroup entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<TaxGroupWithRate>> GetBySupplyTypeWithRatesAsync(string supplyType)
        {
          return await _repository.GetBySupplyTypeWithRatesAsync(supplyType);
        }
  }
}
