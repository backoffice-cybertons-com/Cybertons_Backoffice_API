using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompanyByIdAsync(int companyId);
    }
}
