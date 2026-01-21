using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PaymentTermService : IPaymentTermService
    {
        private readonly IPaymentTermRepository _repository;

        public PaymentTermService(IPaymentTermRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PaymentTerm>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PaymentTerm?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(PaymentTerm entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(PaymentTerm entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
