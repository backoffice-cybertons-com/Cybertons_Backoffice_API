using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class UnitRepository : BaseDAL, IUnitRepository
    {
        public UnitRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Unit>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Unit_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Unit
            {
                UnitId = Convert.ToInt32(reader["UnitId"]),
                UnitName = reader["UnitName"]?.ToString(),
                UQC = reader["UQC"]?.ToString()
            });
        }

        public async Task<Unit?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Unit_Master", 
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@UnitId", id);
                },
                reader => new Unit
                {
                    UnitId = Convert.ToInt32(reader["UnitId"]),
                    UnitName = reader["UnitName"]?.ToString(),
                    UQC = reader["UQC"]?.ToString()
                });
        }

        public async Task InsertAsync(Unit unit)
        {
            await ExecuteNonQueryAsync("sp_Unit_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@UnitName", unit.UnitName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UQC", unit.UQC ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Unit unit)
        {
            await ExecuteNonQueryAsync("sp_Unit_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@UnitId", unit.UnitId);
                cmd.Parameters.AddWithValue("@UnitName", unit.UnitName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UQC", unit.UQC ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Unit_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@UnitId", id);
            });
        }
    }
}
