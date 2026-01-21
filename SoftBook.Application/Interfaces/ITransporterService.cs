using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ITransporterService
    {
        Task<List<Transporter>> GetAllAsync();
        Task<Transporter?> GetByIdAsync(int id);
        Task InsertAsync(Transporter entity);
        Task UpdateAsync(Transporter entity);
        Task DeleteAsync(int id);
    }
}
