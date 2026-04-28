using System.Threading.Tasks;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ICompanyProfileService
    {
        Task<CompanyProfile?> GetByCompanyIdAsync(int companyId);
        Task<DbResult> UpsertAsync(CompanyProfile profile);
    }
}
