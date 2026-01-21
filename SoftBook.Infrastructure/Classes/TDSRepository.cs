using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class TDSRepository : BaseDAL, ITDSRepository
    {
        public TDSRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<TDS>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_TDS_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new TDS
            {
                TDSId = Convert.ToInt32(reader["TDSId"]),
                TaxName = reader["TaxName"]?.ToString() ?? string.Empty,
                Rate = Convert.ToDecimal(reader["Rate"]),
                Section = reader["Section"]?.ToString() ?? string.Empty,
                IsHigherRate = reader["IsHigherRate"] != DBNull.Value,
                StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null
            });
        }

        public async Task<TDS?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_TDS_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@TDSId", id);
                },
                reader => new TDS
                {
                    TDSId = Convert.ToInt32(reader["TDSId"]),
                    TaxName = reader["TaxName"]?.ToString() ?? string.Empty,
                    Rate = Convert.ToDecimal(reader["Rate"]),
                    Section = reader["Section"]?.ToString() ?? string.Empty,
                    IsHigherRate = reader["IsHigherRate"] != DBNull.Value,
                    StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                    EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null
                });
        }

        public async Task InsertAsync(TDS item)
        {
            await ExecuteNonQueryAsync("sp_TDS_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@TaxName", item.TaxName);
                cmd.Parameters.AddWithValue("@Rate", item.Rate);
                cmd.Parameters.AddWithValue("@Section", item.Section);
                cmd.Parameters.AddWithValue("@IsHigherRate", item.IsHigherRate);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(TDS item)
        {
            await ExecuteNonQueryAsync("sp_TDS_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@TDSId", item.TDSId);
                cmd.Parameters.AddWithValue("@TaxName", item.TaxName);
                cmd.Parameters.AddWithValue("@Rate", item.Rate);
                cmd.Parameters.AddWithValue("@Section", item.Section);
                cmd.Parameters.AddWithValue("@IsHigherRate", item.IsHigherRate);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_TDS_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@TDSId", id);
            });
        }
    }
}
