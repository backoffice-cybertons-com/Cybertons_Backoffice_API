using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;
using Npgsql;
using System.Data;

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
            // Postgres function: sp_getcompanybyid must be called with SELECT * FROM
            string query = "SELECT * FROM sp_getcompanybyid(p_company_id := @p_company_id)";

            return await ExecuteSingleReaderAsync(query,
                cmd => 
                { 
                    cmd.Parameters.Add(new NpgsqlParameter("p_company_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = companyId }); 
                },
                reader => 
                {
                    return new Company
                    {
                        Id = SafeGetInt(reader, "company_id"),
                        Name = SafeGetString(reader, "company_name"),
                        ConnectionString = SafeGetString(reader, "connection_string")
                    };
                }, CommandType.Text);
        }
    }
}
