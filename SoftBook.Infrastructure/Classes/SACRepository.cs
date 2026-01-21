using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class SACRepository : BaseDAL, ISACRepository
    {
        public SACRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<SAC>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_SAC_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new SAC
            {
                SACId = Convert.ToInt32(reader["SACId"]),
                SACCode = reader["SACCode"]?.ToString(),
                Description = reader["Description"]?.ToString(),
                TaxRate = reader["TaxRate"] != DBNull.Value ? Convert.ToDecimal(reader["TaxRate"]) : null
            });
        }

        public async Task<SAC?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_SAC_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@SACId", id);
                },
                reader => new SAC
                {
                    SACId = Convert.ToInt32(reader["SACId"]),
                    SACCode = reader["SACCode"]?.ToString(),
                    Description = reader["Description"]?.ToString(),
                    TaxRate = reader["TaxRate"] != DBNull.Value ? Convert.ToDecimal(reader["TaxRate"]) : null
                });
        }

        public async Task InsertAsync(SAC item)
        {
            await ExecuteNonQueryAsync("sp_SAC_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@SACCode", item.SACCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxRate", item.TaxRate ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(SAC item)
        {
            await ExecuteNonQueryAsync("sp_SAC_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@SACId", item.SACId);
                cmd.Parameters.AddWithValue("@SACCode", item.SACCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxRate", item.TaxRate ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_SAC_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@SACId", id);
            });
        }
    }
}
