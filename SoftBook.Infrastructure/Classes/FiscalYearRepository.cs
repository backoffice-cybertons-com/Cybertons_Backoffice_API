using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class FiscalYearRepository : BaseDAL, IFiscalYearRepository
    {
        public FiscalYearRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<FiscalYear>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_FiscalYear_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new FiscalYear
            {
                FiscalYearId = Convert.ToInt32(reader["FiscalYearId"]),
                StartDate = Convert.ToDateTime(reader["StartDate"]),
                EndDate = Convert.ToDateTime(reader["EndDate"]),
                IsCurrent = reader["IsCurrent"] != DBNull.Value ? Convert.ToBoolean(reader["IsCurrent"]) : null
            });
        }

        public async Task<FiscalYear?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_FiscalYear_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@FiscalYearId", id);
                },
                reader => new FiscalYear
                {
                    FiscalYearId = Convert.ToInt32(reader["FiscalYearId"]),
                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                    IsCurrent = reader["IsCurrent"] != DBNull.Value ? Convert.ToBoolean(reader["IsCurrent"]) : null
                });
        }

        public async Task InsertAsync(FiscalYear item)
        {
            await ExecuteNonQueryAsync("sp_FiscalYear_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate);
                cmd.Parameters.AddWithValue("@IsCurrent", item.IsCurrent ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(FiscalYear item)
        {
            await ExecuteNonQueryAsync("sp_FiscalYear_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@FiscalYearId", item.FiscalYearId);
                cmd.Parameters.AddWithValue("@StartDate", item.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", item.EndDate);
                cmd.Parameters.AddWithValue("@IsCurrent", item.IsCurrent ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_FiscalYear_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@FiscalYearId", id);
            });
        }
    }
}
