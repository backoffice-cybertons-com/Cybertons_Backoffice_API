using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class CompanyRepository : BaseDAL, ICompanyRepository
    {
        public CompanyRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) 
        {
            // Always use MasterConnection (or DefaultConnection as fallback) for company lookups
            var masterConn = configuration.GetConnectionString("MasterConnection") 
                          ?? configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(masterConn))
            {
                _connectionString = masterConn;
            }
        }

        protected override string GetConnectionString()
        {
            // Company repository must ALWAYS hit the master database (CompanyMaster)
            return _connectionString;
        }

        public async Task<Company?> GetCompanyByIdAsync(int companyId)
        {
            return await ExecuteSingleReaderAsync("sp_GetCompanyById",
                cmd => { cmd.Parameters.AddWithValue("@CompanyId", companyId); },
                reader => new Company
                {
                    Id = Convert.ToInt32(reader["CompanyId"]),
                    Name = reader["CompanyName"]?.ToString() ?? string.Empty,
                    ConnectionString = reader["ConnectionString"]?.ToString() ?? string.Empty
                });
        }
    }
}
