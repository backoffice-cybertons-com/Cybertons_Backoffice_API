using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class TCSRepository : BaseDAL, ITCSRepository
    {
        public TCSRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<TCS>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_TCS_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new TCS
            {
                TCSId = Convert.ToInt32(reader["TCSId"]),
                Rate = Convert.ToDecimal(reader["Rate"]),
                TaxName = reader["TaxName"]?.ToString() ?? string.Empty,
                NatureOfCollection = reader["NatureOfCollection"]?.ToString() ?? string.Empty,
                IsHigherRate = reader["IsHigherRate"] != DBNull.Value && Convert.ToBoolean(reader["IsHigherRate"]),
                StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null
            });
        }

        public async Task<TCS?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_TCS_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@TCSId", id);
                },
                reader => new TCS
                {
                    TCSId = Convert.ToInt32(reader["TCSId"]),
                    Rate = Convert.ToDecimal(reader["Rate"]),
                    TaxName = reader["TaxName"]?.ToString() ?? string.Empty,
                    NatureOfCollection = reader["NatureOfCollection"]?.ToString() ?? string.Empty,
                    IsHigherRate = reader["IsHigherRate"] != DBNull.Value && Convert.ToBoolean(reader["IsHigherRate"]),
                    StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : null,
                    EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToDateTime(reader["EndDate"]) : null
                });
        }

        public async Task InsertAsync(TCS item)
        {
            await ExecuteNonQueryAsync("sp_TCS_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@Rate", item.Rate);
                cmd.Parameters.AddWithValue("@TaxName", item.TaxName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NatureOfCollection", item.NatureOfCollection);
                cmd.Parameters.AddWithValue("@IsHigherRate", item.IsHigherRate);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(TCS item)
        {
            await ExecuteNonQueryAsync("sp_TCS_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@TCSId", item.TCSId);
                cmd.Parameters.AddWithValue("@Rate", item.Rate);
                cmd.Parameters.AddWithValue("@TaxName", item.TaxName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NatureOfCollection", item.NatureOfCollection);
                cmd.Parameters.AddWithValue("@IsHigherRate", item.IsHigherRate);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_TCS_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@TCSId", id);
            });
        }
    }
}
