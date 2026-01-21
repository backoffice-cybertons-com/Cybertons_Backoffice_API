using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class RoleRepository : BaseDAL, IRoleRepository
    {
        public RoleRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Role>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Role_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Role
            {
                RoleId = Convert.ToInt32(reader["RoleId"]),
                RoleName = reader["RoleName"]?.ToString()
            });
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Role_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@RoleId", id);
                },
                reader => new Role
                {
                    RoleId = Convert.ToInt32(reader["RoleId"]),
                    RoleName = reader["RoleName"]?.ToString()
                });
        }

        public async Task InsertAsync(Role item)
        {
            await ExecuteNonQueryAsync("sp_Role_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@RoleName", item.RoleName ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Role item)
        {
            await ExecuteNonQueryAsync("sp_Role_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@RoleId", item.RoleId);
                cmd.Parameters.AddWithValue("@RoleName", item.RoleName ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Role_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@RoleId", id);
            });
        }
    }
}
