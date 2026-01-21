using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class TaxService : ITaxService
    {
        private readonly ITaxRepository _repository;

        public TaxService(ITaxRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Tax>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Tax?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(Tax entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(Tax entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
