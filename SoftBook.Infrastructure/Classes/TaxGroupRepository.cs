using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using Microsoft.Data.SqlClient;

namespace SoftBook.Infrastructure.Repositories.Classes
{
  public class TaxGroupRepository : BaseDAL, ITaxGroupRepository
  {
    public TaxGroupRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

    public async Task<List<TaxGroupWithRate>> GetBySupplyTypeWithRatesAsync(string supplyType)
    {
      // SQL query to get Tax Groups with calculated total rates
      // Joins TabTaxGroupMaster -> TabTaxGroupDetail -> TabTaxMaster
      // Groups by TaxGroupId and sums RatePercent
      const string sqlQuery = @"
                SELECT 
                    g.TaxGroupId,
                    g.GroupName,
                    g.SupplyType,
                    ISNULL(SUM(t.RatePercent), 0) AS TotalRatePercent
                FROM TabTaxGroupMaster g
                LEFT JOIN TabTaxGroupDetail d ON g.TaxGroupId = d.TaxGroupId
                LEFT JOIN TabTaxMaster t ON d.TaxId = t.TaxId
                WHERE g.SupplyType = @SupplyType
                GROUP BY g.TaxGroupId, g.GroupName, g.SupplyType
                ORDER BY TotalRatePercent ASC";

      return await ExecuteRawQueryAsync(sqlQuery,
          cmd => cmd.Parameters.AddWithValue("@SupplyType", supplyType),
          reader => new TaxGroupWithRate
          {
            TaxGroupId = Convert.ToInt32(reader["TaxGroupId"]),
            GroupName = reader["GroupName"]?.ToString(),
            SupplyType = reader["SupplyType"]?.ToString(),
            TotalRatePercent = reader["TotalRatePercent"] != DBNull.Value ? Convert.ToDecimal(reader["TotalRatePercent"]) : 0
          });
    }

    public async Task<List<TaxGroup>> GetAllAsync()
    {
      return await ExecuteReaderAsync("sp_TaxGroup_Master",
          cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
          reader => new TaxGroup
          {
            TaxGroupId = Convert.ToInt32(reader["TaxGroupId"]),
            GroupName = reader["GroupName"]?.ToString(),
            Description = reader["Description"]?.ToString(),
            SupplyType = reader["SupplyType"]?.ToString()
          });
    }

    public async Task<TaxGroup?> GetByIdAsync(int id)
    {
      return await ExecuteSingleReaderAsync("sp_TaxGroup_Master",
          cmd =>
          {
            cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
            cmd.Parameters.AddWithValue("@TaxGroupId", id);
          },
          reader => new TaxGroup
          {
            TaxGroupId = Convert.ToInt32(reader["TaxGroupId"]),
            GroupName = reader["GroupName"]?.ToString(),
            Description = reader["Description"]?.ToString(),
            SupplyType = reader["SupplyType"]?.ToString()
          });
    }

    public async Task InsertAsync(TaxGroup item)
    {
      await ExecuteNonQueryAsync("sp_TaxGroup_Master", cmd =>
      {
        cmd.Parameters.AddWithValue("@Action", "INSERT");
        cmd.Parameters.AddWithValue("@GroupName", item.GroupName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@SupplyType", item.SupplyType ?? (object)DBNull.Value);
      });
    }

    public async Task UpdateAsync(TaxGroup item)
    {
      await ExecuteNonQueryAsync("sp_TaxGroup_Master", cmd =>
      {
        cmd.Parameters.AddWithValue("@Action", "UPDATE");
        cmd.Parameters.AddWithValue("@TaxGroupId", item.TaxGroupId);
        cmd.Parameters.AddWithValue("@GroupName", item.GroupName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@Description", item.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@SupplyType", item.SupplyType ?? (object)DBNull.Value);
      });
    }

    public async Task DeleteAsync(int id)
    {
      try
      {
        await ExecuteNonQueryAsync("sp_TaxGroup_Master", cmd =>
        {
          cmd.Parameters.AddWithValue("@Action", "DELETE");
          cmd.Parameters.AddWithValue("@TaxGroupId", id);
        });
      }
      catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 50000)
      {
        throw new InvalidOperationException(ex.Message);
      }
    }
  }
}
