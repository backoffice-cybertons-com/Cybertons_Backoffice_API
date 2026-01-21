using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class HSNService : IHSNService
    {
        private readonly IHSNRepository _repository;

        public HSNService(IHSNRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<HSN>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<HSN?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(HSN entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(HSN entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
