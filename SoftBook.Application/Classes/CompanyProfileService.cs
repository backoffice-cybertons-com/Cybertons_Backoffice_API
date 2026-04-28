using System.Threading.Tasks;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Application.Services.Classes
{
    public class CompanyProfileService : ICompanyProfileService
    {
        private readonly ICompanyProfileRepository _repository;

        public CompanyProfileService(ICompanyProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<CompanyProfile?> GetByCompanyIdAsync(int companyId)
        {
            return await _repository.GetByCompanyIdAsync(companyId);
        }

        public async Task<DbResult> UpsertAsync(CompanyProfile profile)
        {
            return await _repository.UpsertAsync(profile);
        }
    }
}
