using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class HSNRepository : BaseDAL, IHSNRepository
    {
        public HSNRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<HSN>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_HSN_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new HSN
            {
                HSNId = Convert.ToInt32(reader["HSNId"]),
                HSNCode = reader["HSNCode"]?.ToString(),
                Description = reader["Description"]?.ToString(),
                TaxRate = reader["TaxRate"] != DBNull.Value ? Convert.ToDecimal(reader["TaxRate"]) : null
            });
        }

        public async Task<HSN?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_HSN_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@HSNId", id);
                },
                reader => new HSN
                {
                    HSNId = Convert.ToInt32(reader["HSNId"]),
                    HSNCode = reader["HSNCode"]?.ToString(),
                    Description = reader["Description"]?.ToString(),
                    TaxRate = reader["TaxRate"] != DBNull.Value ? Convert.ToDecimal(reader["TaxRate"]) : null
                });
        }

        public async Task InsertAsync(HSN hsn)
        {
            await ExecuteNonQueryAsync("sp_HSN_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@HSNCode", hsn.HSNCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", hsn.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxRate", hsn.TaxRate ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(HSN hsn)
        {
            await ExecuteNonQueryAsync("sp_HSN_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@HSNId", hsn.HSNId);
                cmd.Parameters.AddWithValue("@HSNCode", hsn.HSNCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", hsn.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxRate", hsn.TaxRate ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_HSN_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@HSNId", id);
            });
        }
    }
}
