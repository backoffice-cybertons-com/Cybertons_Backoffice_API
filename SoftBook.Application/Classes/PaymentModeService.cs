using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.Application.Services.Classes
{
    public class PaymentModeService : IPaymentModeService
    {
        private readonly IPaymentModeRepository _repository;

        public PaymentModeService(IPaymentModeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PaymentMode>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PaymentMode?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(PaymentMode entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(PaymentMode entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
