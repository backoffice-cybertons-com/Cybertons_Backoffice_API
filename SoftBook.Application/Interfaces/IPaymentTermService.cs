using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPaymentTermService
    {
        Task<List<PaymentTerm>> GetAllAsync();
        Task<PaymentTerm?> GetByIdAsync(int id);
        Task InsertAsync(PaymentTerm entity);
        Task UpdateAsync(PaymentTerm entity);
        Task DeleteAsync(int id);
    }
}
