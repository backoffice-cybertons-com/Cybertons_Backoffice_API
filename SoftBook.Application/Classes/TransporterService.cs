using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class TransporterService : ITransporterService
    {
        private readonly ITransporterRepository _repository;

        public TransporterService(ITransporterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Transporter>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Transporter?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(Transporter entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(Transporter entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
