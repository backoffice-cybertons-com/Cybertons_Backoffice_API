using SoftBook.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class UserRepository : BaseDAL, IUserRepository
    {
        public UserRepository(IConfiguration configuration, ITenantProvider tenantProvider) : base(configuration, tenantProvider) { }

        public async Task<User?> ValidateUserAsync(string username, string password, int companyId)
        {
            return await ExecuteSingleReaderAsync("sp_ValidateUser",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", password);
                },
                reader => new User
                {
                    Id = Convert.ToInt32(reader["UserId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty
                });
        }

        public async Task<List<SidebarItem>> GetSidebarPermissionsAsync(int userId)
        {
            return await ExecuteReaderAsync("USP_GetUserSidebarAndPermissions",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                },
                reader => new SidebarItem
                {
                    RoleId = Convert.ToInt32(reader["RoleId"]),
                    RoleName = reader["RoleName"]?.ToString() ?? string.Empty,
                    TabId = Convert.ToInt32(reader["TabId"]),
                    TabName = reader["TabName"]?.ToString() ?? string.Empty,
                    TabUrl = reader["TabUrl"]?.ToString() ?? string.Empty,
                    ParentId = reader["ParentId"] != DBNull.Value ? Convert.ToInt32(reader["ParentId"]) : (int?)null,
                    IconPath = reader["IconPath"]?.ToString() ?? string.Empty,
                    SortOrder = Convert.ToInt32(reader["SortOrder"]),
                    CanView = Convert.ToBoolean(reader["CanView"]),
                    CanAdd = Convert.ToBoolean(reader["CanAdd"]),
                    CanEdit = Convert.ToBoolean(reader["CanEdit"]),
                    CanDelete = Convert.ToBoolean(reader["CanDelete"])
                });
        }

        public async Task<User?> InsertUserAsync(string username, string passwordHash, string email, string fullName = null, string phone = null)
        {
            return await ExecuteSingleReaderAsync("sp_User_Register",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@FullName", (object)fullName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", (object)phone ?? DBNull.Value);
                    
                    var userIdParam = new SqlParameter("@UserId", System.Data.SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(userIdParam);
                },
                reader => new User
                {
                    Id = Convert.ToInt32(reader["UserId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    PasswordHash = string.Empty // Don't return password hash
                });
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await ExecuteSingleReaderAsync("sp_User_GetByEmail",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                },
                reader => new User
                {
                    Id = Convert.ToInt32(reader["UserId"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty
                });
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPasswordHash)
        {
            try
            {
                using var connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();
                
                using var cmd = new SqlCommand("sp_User_UpdatePassword", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);
                
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await ExecuteReaderAsync("sp_User_Master",
                cmd => cmd.Parameters.AddWithValue("@Action", "SELECT"),
                reader => new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    FullName = reader["FullName"]?.ToString(),
                    Phone = reader["Phone"]?.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_User_Master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@UserId", id);
                },
                reader => new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    FullName = reader["FullName"]?.ToString(),
                    Phone = reader["Phone"]?.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                });
        }

        public async Task InsertAsync(User user)
        {
            await ExecuteNonQueryAsync("sp_User_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@UserId", user.Id);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@FullName", (object)user.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            });
        }

        public async Task UpdateAsync(User user)
        {
            await ExecuteNonQueryAsync("sp_User_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@UserId", user.Id);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@FullName", (object)user.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_User_Master", cmd =>
            {
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@UserId", id);
            });
        }
    }
}
