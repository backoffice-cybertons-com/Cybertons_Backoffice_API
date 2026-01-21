using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class GodownService : IGodownService
    {
        private readonly IGodownRepository _repository;

        public GodownService(IGodownRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Godown>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Godown?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(Godown entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(Godown entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
