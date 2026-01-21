using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IFiscalYearService
    {
        Task<List<FiscalYear>> GetAllAsync();
        Task<FiscalYear?> GetByIdAsync(int id);
        Task InsertAsync(FiscalYear entity);
        Task UpdateAsync(FiscalYear entity);
        Task DeleteAsync(int id);
    }
}
