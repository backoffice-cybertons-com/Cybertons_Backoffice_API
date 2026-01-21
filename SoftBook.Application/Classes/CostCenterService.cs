using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class CostCenterService : ICostCenterService
    {
        private readonly ICostCenterRepository _repository;

        public CostCenterService(ICostCenterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CostCenter>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CostCenter?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(CostCenter entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(CostCenter entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
