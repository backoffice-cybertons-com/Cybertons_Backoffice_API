using SoftBook.Domain.Entities;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Company?> GetCompanyByIdAsync(int companyId);
    }
}
