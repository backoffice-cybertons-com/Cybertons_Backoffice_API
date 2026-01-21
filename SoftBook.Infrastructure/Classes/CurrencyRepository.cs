using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class CurrencyRepository : BaseDAL, ICurrencyRepository
    {
        public CurrencyRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Currency>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Currency_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Currency
            {
                CurrencyId = Convert.ToInt32(reader["CurrencyId"]),
                CurrencyCode = reader["CurrencyCode"]?.ToString(),
                CurrencySymbol = reader["CurrencySymbol"]?.ToString(),
                CurrencyName = reader["CurrencyName"]?.ToString(),
                DecimalPlaces = reader["DecimalPlaces"] != DBNull.Value ? Convert.ToByte(reader["DecimalPlaces"]) : null,
                Format = reader["Format"]?.ToString()
            });
        }

        public async Task<Currency?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Currency_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@CurrencyId", id);
                },
                reader => new Currency
                {
                    CurrencyId = Convert.ToInt32(reader["CurrencyId"]),
                    CurrencyCode = reader["CurrencyCode"]?.ToString(),
                    CurrencySymbol = reader["CurrencySymbol"]?.ToString(),
                    CurrencyName = reader["CurrencyName"]?.ToString(),
                    DecimalPlaces = reader["DecimalPlaces"] != DBNull.Value ? Convert.ToByte(reader["DecimalPlaces"]) : null,
                    Format = reader["Format"]?.ToString()
                });
        }

        public async Task InsertAsync(Currency currency)
        {
            await ExecuteNonQueryAsync("sp_Currency_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@CurrencyCode", currency.CurrencyCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrencySymbol", currency.CurrencySymbol ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrencyName", currency.CurrencyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DecimalPlaces", currency.DecimalPlaces ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Format", currency.Format ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Currency currency)
        {
            await ExecuteNonQueryAsync("sp_Currency_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@CurrencyId", currency.CurrencyId);
                cmd.Parameters.AddWithValue("@CurrencyCode", currency.CurrencyCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrencySymbol", currency.CurrencySymbol ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrencyName", currency.CurrencyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DecimalPlaces", currency.DecimalPlaces ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Format", currency.Format ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Currency_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@CurrencyId", id);
            });
        }
    }
}
