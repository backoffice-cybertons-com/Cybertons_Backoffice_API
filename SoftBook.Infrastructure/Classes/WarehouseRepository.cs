using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class WarehouseRepository : BaseDAL, IWarehouseRepository
    {
        public WarehouseRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Warehouse>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Warehouse_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Warehouse
            {
                WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                WarehouseName = reader["WarehouseName"]?.ToString() ?? string.Empty,
                BranchId = reader["BranchId"] != DBNull.Value ? Convert.ToInt32(reader["BranchId"]) : null,
                Address = reader["Address"]?.ToString()
            });
        }

        public async Task<Warehouse?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Warehouse_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@WarehouseId", id);
                },
                reader => new Warehouse
                {
                    WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                    WarehouseName = reader["WarehouseName"]?.ToString() ?? string.Empty,
                    BranchId = reader["BranchId"] != DBNull.Value ? Convert.ToInt32(reader["BranchId"]) : null,
                    Address = reader["Address"]?.ToString()
                });
        }

        public async Task InsertAsync(Warehouse warehouse)
        {
            await ExecuteNonQueryAsync("sp_Warehouse_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@WarehouseName", warehouse.WarehouseName);
                cmd.Parameters.AddWithValue("@BranchId", warehouse.BranchId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", warehouse.Address ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Warehouse warehouse)
        {
            await ExecuteNonQueryAsync("sp_Warehouse_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@WarehouseId", warehouse.WarehouseId);
                cmd.Parameters.AddWithValue("@WarehouseName", warehouse.WarehouseName);
                cmd.Parameters.AddWithValue("@BranchId", warehouse.BranchId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", warehouse.Address ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Warehouse_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@WarehouseId", id);
            });
        }
    }
}
