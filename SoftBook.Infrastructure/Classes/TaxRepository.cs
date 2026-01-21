using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using Microsoft.Data.SqlClient;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class TaxRepository : BaseDAL, ITaxRepository
    {
        public TaxRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Tax>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Tax_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Tax
            {
                TaxId = Convert.ToInt32(reader["TaxId"]),
                TaxName = reader["TaxName"]?.ToString(),
                RatePercent = reader["RatePercent"] != DBNull.Value ? Convert.ToDecimal(reader["RatePercent"]) : null,
                TaxType = reader["TaxType"]?.ToString()
            });
        }

        public async Task<Tax?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Tax_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@TaxId", id);
                },
                reader => new Tax
                {
                    TaxId = Convert.ToInt32(reader["TaxId"]),
                    TaxName = reader["TaxName"]?.ToString(),
                    RatePercent = reader["RatePercent"] != DBNull.Value ? Convert.ToDecimal(reader["RatePercent"]) : null,
                    TaxType = reader["TaxType"]?.ToString()
                });
        }

        public async Task InsertAsync(Tax tax)
        {
            await ExecuteNonQueryAsync("sp_Tax_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@TaxName", tax.TaxName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RatePercent", tax.RatePercent ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxType", tax.TaxType ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Tax tax)
        {
            await ExecuteNonQueryAsync("sp_Tax_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@TaxId", tax.TaxId);
                cmd.Parameters.AddWithValue("@TaxName", tax.TaxName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RatePercent", tax.RatePercent ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxType", tax.TaxType ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await ExecuteNonQueryAsync("sp_Tax_Master", cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@TaxId", id);
                });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 50000)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
