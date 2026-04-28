using System.Threading.Tasks;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface ICompanyProfileRepository
    {
        Task<CompanyProfile?> GetByCompanyIdAsync(int companyId);
        Task<DbResult> UpsertAsync(CompanyProfile profile);
    }
}
