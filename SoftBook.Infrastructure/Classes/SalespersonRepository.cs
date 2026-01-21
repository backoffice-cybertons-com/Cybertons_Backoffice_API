using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class SalespersonRepository : BaseDAL, ISalespersonRepository
    {
        public SalespersonRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Salesperson>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Salesperson_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Salesperson
            {
                SalespersonId = Convert.ToInt32(reader["SalespersonId"]),
                SalespersonName = reader["SalespersonName"]?.ToString(),
                Email = reader["Email"]?.ToString()
            });
        }

        public async Task<Salesperson?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Salesperson_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@SalespersonId", id);
                },
                reader => new Salesperson
                {
                    SalespersonId = Convert.ToInt32(reader["SalespersonId"]),
                    SalespersonName = reader["SalespersonName"]?.ToString(),
                    Email = reader["Email"]?.ToString()
                });
        }

        public async Task InsertAsync(Salesperson item)
        {
            await ExecuteNonQueryAsync("sp_Salesperson_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@SalespersonName", item.SalespersonName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", item.Email ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Salesperson item)
        {
            await ExecuteNonQueryAsync("sp_Salesperson_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@SalespersonId", item.SalespersonId);
                cmd.Parameters.AddWithValue("@SalespersonName", item.SalespersonName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", item.Email ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Salesperson_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@SalespersonId", id);
            });
        }
    }
}
