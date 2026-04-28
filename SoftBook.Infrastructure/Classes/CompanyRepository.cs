using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
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
        public async Task<List<Company>> GetAllAsync()
        {
            // Fix: tab_company_master uses 'is_enabled' instead of 'delete_flag'
            string query = "SELECT company_id, company_name, connection_string FROM tab_company_master WHERE is_enabled = true";

            return await ExecuteReaderAsync(query,
                cmd => { },
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
