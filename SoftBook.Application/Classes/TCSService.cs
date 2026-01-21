using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class TCSService : ITCSService
    {
        private readonly ITCSRepository _repository;

        public TCSService(ITCSRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TCS>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TCS?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(TCS entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(TCS entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
