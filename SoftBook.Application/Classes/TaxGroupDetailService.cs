using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class TaxGroupDetailService : ITaxGroupDetailService
    {
        private readonly ITaxGroupDetailRepository _repository;

        public TaxGroupDetailService(ITaxGroupDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaxGroupDetail>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TaxGroupDetail?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<TaxGroupDetail>> GetByTaxGroupIdAsync(int taxGroupId)
        {
            return await _repository.GetByTaxGroupIdAsync(taxGroupId);
        }

        public async Task InsertAsync(TaxGroupDetail entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(TaxGroupDetail entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}


