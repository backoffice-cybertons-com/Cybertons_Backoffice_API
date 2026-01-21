using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class SalespersonService : ISalespersonService
    {
        private readonly ISalespersonRepository _repository;

        public SalespersonService(ISalespersonRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Salesperson>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Salesperson?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(Salesperson entity)
        {
            // Add any business logic/validation here before calling repository
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(Salesperson entity)
        {
            // Add any business logic/validation here before calling repository
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            // Add any business logic/validation here before calling repository
            await _repository.DeleteAsync(id);
        }
    }
}
