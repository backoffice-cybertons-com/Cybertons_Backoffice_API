using Microsoft.Extensions.Configuration;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class BranchRepository : BaseDAL, IBranchRepository
    {
        public BranchRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<List<Branch>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_Branch_Master", 
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new Branch
            {
                BranchId = Convert.ToInt32(reader["BranchId"]),
                BranchName = reader["BranchName"]?.ToString() ?? string.Empty,
                Address = reader["Address"]?.ToString(),
                City = reader["City"]?.ToString(),
                State = reader["State"]?.ToString(),
                Country = reader["Country"]?.ToString(),
                Phone = reader["Phone"]?.ToString(),
                IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : null
            });
        }

        public async Task<Branch?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_Branch_Master",
                cmd => 
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@BranchId", id);
                },
                reader => new Branch
                {
                    BranchId = Convert.ToInt32(reader["BranchId"]),
                    BranchName = reader["BranchName"]?.ToString() ?? string.Empty,
                    Address = reader["Address"]?.ToString(),
                    City = reader["City"]?.ToString(),
                    State = reader["State"]?.ToString(),
                    Country = reader["Country"]?.ToString(),
                    Phone = reader["Phone"]?.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : null
                });
        }

        public async Task InsertAsync(Branch branch)
        {
            await ExecuteNonQueryAsync("sp_Branch_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@BranchName", branch.BranchName);
                cmd.Parameters.AddWithValue("@Address", branch.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", branch.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", branch.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", branch.Country ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", branch.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", branch.IsActive ?? (object)DBNull.Value);
            });
        }

        public async Task UpdateAsync(Branch branch)
        {
            await ExecuteNonQueryAsync("sp_Branch_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@BranchId", branch.BranchId);
                cmd.Parameters.AddWithValue("@BranchName", branch.BranchName);
                cmd.Parameters.AddWithValue("@Address", branch.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", branch.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@State", branch.State ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", branch.Country ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", branch.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", branch.IsActive ?? (object)DBNull.Value);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_Branch_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@BranchId", id);
            });
        }
    }
}
