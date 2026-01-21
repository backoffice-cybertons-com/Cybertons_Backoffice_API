using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class GodownRepository : BaseDAL, IGodownRepository
    {
        public GodownRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Godown>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Godown_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Godown
            {
                GodownId = Convert.ToInt32(reader["GodownId"]),
                WarehouseId = reader["WarehouseId"] != DBNull.Value ? Convert.ToInt32(reader["WarehouseId"]) : null,
                GodownName = reader["GodownName"]?.ToString()
            });
        }

        public async Task<Godown?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Godown_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@GodownId", id);
                },
                reader => new Godown
                {
                    GodownId = Convert.ToInt32(reader["GodownId"]),
                    WarehouseId = reader["WarehouseId"] != DBNull.Value ? Convert.ToInt32(reader["WarehouseId"]) : null,
                    GodownName = reader["GodownName"]?.ToString()
                });
        }

        public async Task InsertAsync(Godown item)
        {
            await ExecuteNonQueryAsync("sp_Godown_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@WarehouseId", item.WarehouseId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GodownName", item.GodownName ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Godown item)
        {
            await ExecuteNonQueryAsync("sp_Godown_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@GodownId", item.GodownId);
                cmd.Parameters.AddWithValue("@WarehouseId", item.WarehouseId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GodownName", item.GodownName ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Godown_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@GodownId", id);
            });
        }
    }
}
