using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IPaymentModeService
    {
        Task<List<PaymentMode>> GetAllAsync();
        Task<PaymentMode?> GetByIdAsync(int id);
        Task InsertAsync(PaymentMode entity);
        Task UpdateAsync(PaymentMode entity);
        Task DeleteAsync(int id);
    }
}
