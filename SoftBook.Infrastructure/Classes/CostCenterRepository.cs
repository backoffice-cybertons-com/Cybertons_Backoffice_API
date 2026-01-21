using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class CostCenterRepository : BaseDAL, ICostCenterRepository
    {
        public CostCenterRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<CostCenter>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_CostCenter_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new CostCenter
            {
                CostCenterId = Convert.ToInt32(reader["CostCenterId"]),
                CostCenterName = reader["CostCenterName"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString()
            });
        }

        public async Task<CostCenter?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_CostCenter_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@CostCenterId", id);
                },
                reader => new CostCenter
                {
                    CostCenterId = Convert.ToInt32(reader["CostCenterId"]),
                    CostCenterName = reader["CostCenterName"]?.ToString() ?? string.Empty,
                    Description = reader["Description"]?.ToString()
                });
        }

        public async Task InsertAsync(CostCenter item)
        {
            await ExecuteNonQueryAsync("sp_CostCenter_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@CostCenterName", item.CostCenterName);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(CostCenter item)
        {
            await ExecuteNonQueryAsync("sp_CostCenter_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@CostCenterId", item.CostCenterId);
                cmd.Parameters.AddWithValue("@CostCenterName", item.CostCenterName);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_CostCenter_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@CostCenterId", id);
            });
        }
    }
}
