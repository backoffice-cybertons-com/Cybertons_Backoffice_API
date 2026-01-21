using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class FiscalYearService : IFiscalYearService
    {
        private readonly IFiscalYearRepository _repository;

        public FiscalYearService(IFiscalYearRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FiscalYear>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<FiscalYear?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(FiscalYear entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(FiscalYear entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
