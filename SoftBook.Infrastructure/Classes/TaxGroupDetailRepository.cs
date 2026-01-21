using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using Microsoft.Data.SqlClient;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class TaxGroupDetailRepository : BaseDAL, ITaxGroupDetailRepository
    {
        public TaxGroupDetailRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<TaxGroupDetail>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_TaxGroupDetail_Master",
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new TaxGroupDetail
                {
                    TaxGroupDetailId = Convert.ToInt32(reader["TaxGroupDetailId"]),
                    TaxGroupId = Convert.ToInt32(reader["TaxGroupId"]),
                    TaxId = Convert.ToInt32(reader["TaxId"]),
                    ApplyOrder = Convert.ToInt32(reader["ApplyOrder"]),
                    GroupName = reader["GroupName"]?.ToString(),
                    TaxName = reader["TaxName"]?.ToString(),
                    RatePercent = reader["RatePercent"] != DBNull.Value ? Convert.ToDecimal(reader["RatePercent"]) : null,
                    TaxType = reader["TaxType"]?.ToString()
                });
        }

        public async Task<TaxGroupDetail?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_TaxGroupDetail_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@TaxGroupDetailId", id);
                },
                reader => new TaxGroupDetail
                {
                    TaxGroupDetailId = Convert.ToInt32(reader["TaxGroupDetailId"]),
                    TaxGroupId = Convert.ToInt32(reader["TaxGroupId"]),
                    TaxId = Convert.ToInt32(reader["TaxId"]),
                    ApplyOrder = Convert.ToInt32(reader["ApplyOrder"])
                });
        }

        public async Task<List<TaxGroupDetail>> GetByTaxGroupIdAsync(int taxGroupId)
        {
            return await ExecuteReaderAsync("sp_TaxGroupDetail_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_GROUP");
                    cmd.Parameters.AddWithValue("@TaxGroupId", taxGroupId);
                },
                reader => new TaxGroupDetail
                {
                    TaxGroupDetailId = Convert.ToInt32(reader["TaxGroupDetailId"]),
                    TaxGroupId = taxGroupId,
                    TaxId = Convert.ToInt32(reader["TaxId"]),
                    ApplyOrder = Convert.ToInt32(reader["ApplyOrder"]),
                    TaxName = reader["TaxName"]?.ToString(),
                    RatePercent = reader["RatePercent"] != DBNull.Value ? Convert.ToDecimal(reader["RatePercent"]) : null,
                    TaxType = reader["TaxType"]?.ToString()
                });
        }

        public async Task InsertAsync(TaxGroupDetail entity)
        {
            try
            {
                await ExecuteNonQueryAsync("sp_TaxGroupDetail_Master", cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@TaxGroupId", entity.TaxGroupId);
                    cmd.Parameters.AddWithValue("@TaxId", entity.TaxId);
                    cmd.Parameters.AddWithValue("@ApplyOrder", entity.ApplyOrder);
                });
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task UpdateAsync(TaxGroupDetail entity)
        {
            try
            {
                await ExecuteNonQueryAsync("sp_TaxGroupDetail_Master", cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "UPDATE");
                    cmd.Parameters.AddWithValue("@TaxGroupDetailId", entity.TaxGroupDetailId);
                    cmd.Parameters.AddWithValue("@TaxGroupId", entity.TaxGroupId);
                    cmd.Parameters.AddWithValue("@TaxId", entity.TaxId);
                    cmd.Parameters.AddWithValue("@ApplyOrder", entity.ApplyOrder);
                });
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_TaxGroupDetail_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@TaxGroupDetailId", id);
            });
        }
    }
}
