using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Classes
{
    public class TenantResolutionService : ITenantResolutionService
    {
        private readonly ICompanyRepository _companyRepository;

        public TenantResolutionService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<string?> GetConnectionStringAsync(int tenantId)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(tenantId);
            return company?.ConnectionString;
        }
    }
}
