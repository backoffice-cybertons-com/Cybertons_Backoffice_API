using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class SACService : ISACService
    {
        private readonly ISACRepository _repository;

        public SACService(ISACRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SAC>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<SAC?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(SAC entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(SAC entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
