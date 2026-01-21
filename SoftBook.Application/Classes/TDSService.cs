using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class TDSService : ITDSService
    {
        private readonly ITDSRepository _repository;

        public TDSService(ITDSRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TDS>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TDS?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(TDS entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(TDS entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
