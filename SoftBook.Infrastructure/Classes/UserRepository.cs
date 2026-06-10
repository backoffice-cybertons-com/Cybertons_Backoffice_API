using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.RepositoryInterfaces;
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
            // Note: In new model, companyId is handled by the connection string/tenant provider context,
            // but we might still pass it for validation if needed.
            // PostgreSQL function name is snake_case: sp_validate_user
            return await ExecuteSingleReaderAsync("sp_validate_user",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_username", username);
                    cmd.Parameters.AddWithValue("p_password_hash", password);
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                },
                reader => new User
                {
                    Id = SafeGetInt(reader, "user_id"),
                    Username = SafeGetString(reader, "username"),
                    Email = SafeGetString(reader, "email"),
                    PasswordHash = SafeGetString(reader, "password_hash"),
                    CompanyId = SafeGetInt(reader, "company_id")
                });
        }

        public async Task<List<SidebarItem>> GetSidebarPermissionsAsync(int userId, int companyId)
        {
            // Function: usp_get_user_sidebar_and_permissions
            return await ExecuteReaderAsync("usp_get_user_sidebar_and_permissions",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_user_id", userId);
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                },
                reader => new SidebarItem
                {
                    RoleId = SafeGetInt(reader, "role_id"),
                    RoleName = SafeGetString(reader, "role_name"),
                    TabId = SafeGetInt(reader, "tab_id"),
                    TabName = SafeGetString(reader, "tab_name"),
                    TabUrl = SafeGetString(reader, "tab_url"),
                    ParentId = reader["parent_id"] != DBNull.Value ? Convert.ToInt32(reader["parent_id"]) : (int?)null,
                    IconPath = SafeGetString(reader, "icon_path"),
                    SortOrder = SafeGetInt(reader, "sort_order"),
                    CanView = SafeGetBool(reader, "can_view") ?? false,
                    CanAdd = SafeGetBool(reader, "can_add") ?? false,
                    CanEdit = SafeGetBool(reader, "can_edit") ?? false,
                    CanDelete = SafeGetBool(reader, "can_delete") ?? false
                });
        }

        public async Task<User?> InsertUserAsync(string username, string passwordHash, string email, string fullName = null, string phone = null, int? roleId = null, int? companyId = null)
        {
            // Function: sp_user_register
            return await ExecuteSingleReaderAsync("sp_user_register",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_username", username);
                    cmd.Parameters.AddWithValue("p_password_hash", passwordHash);
                    cmd.Parameters.AddWithValue("p_email", email);
                    cmd.Parameters.AddWithValue("p_full_name", (object)fullName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_phone", (object)phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_role_id", (object)roleId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("p_company_id", (object)companyId ?? 0);
                    // Output parameter logic is different in PG, usually we return result set.
                    // The refined API should return the new user as a row.
                },
                reader => new User
                {
                    Id = SafeGetInt(reader, "user_id"),
                    Username = SafeGetString(reader, "username"),
                    Email = SafeGetString(reader, "email"),
                    PasswordHash = "" // Don't return hash
                });
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await ExecuteSingleReaderAsync("sp_user_get_by_email",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_email", email);
                },
                reader => new User
                {
                    Id = SafeGetInt(reader, "user_id"),
                    Username = SafeGetString(reader, "username"),
                    Email = SafeGetString(reader, "email"),
                    PasswordHash = SafeGetString(reader, "password_hash")
                });
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPasswordHash)
        {
            try
            {
                // Use BaseDAL helper instead of raw connection
                await ExecuteNonQueryAsync("sp_user_update_password", cmd =>
                {
                    cmd.Parameters.AddWithValue("p_email", email);
                    cmd.Parameters.AddWithValue("p_new_password_hash", newPasswordHash);
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            // Universal Master access for User
            return await ExecuteReaderAsync("sp_user_master",
                cmd => cmd.Parameters.AddWithValue("p_action", "SELECT"),
                reader => new User
                {
                    Id = SafeGetInt(reader, "user_id"),
                    Username = SafeGetString(reader, "username"),
                    Email = SafeGetString(reader, "email"),
                    FullName = SafeGetString(reader, "full_name"),
                    Phone = SafeGetString(reader, "phone"),
                    IsActive = SafeGetBool(reader, "is_active") ?? true
                });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await ExecuteSingleReaderAsync("sp_user_master",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_action", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("p_user_id", id);
                },
                reader => new User
                {
                    Id = SafeGetInt(reader, "user_id"),
                    Username = SafeGetString(reader, "username"),
                    Email = SafeGetString(reader, "email"),
                    FullName = SafeGetString(reader, "full_name"),
                    Phone = SafeGetString(reader, "phone"),
                    IsActive = SafeGetBool(reader, "is_active") ?? true
                });
        }

        public async Task InsertAsync(User user)
        {
            await ExecuteNonQueryAsync("sp_user_master", cmd =>
            {
                cmd.Parameters.AddWithValue("p_action", "INSERT");
                cmd.Parameters.AddWithValue("p_user_id", user.Id);
                cmd.Parameters.AddWithValue("p_email", user.Email);
                cmd.Parameters.AddWithValue("p_full_name", (object)user.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_is_active", user.IsActive);
            });
        }

        public async Task UpdateAsync(User user)
        {
            await ExecuteNonQueryAsync("sp_user_master", cmd =>
            {
                cmd.Parameters.AddWithValue("p_action", "UPDATE");
                cmd.Parameters.AddWithValue("p_user_id", user.Id);
                cmd.Parameters.AddWithValue("p_email", user.Email);
                cmd.Parameters.AddWithValue("p_full_name", (object)user.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("p_is_active", user.IsActive);
            });
        }

        public async Task DeleteAsync(int id)
        {
            await ExecuteNonQueryAsync("sp_user_master", cmd =>
            {
                cmd.Parameters.AddWithValue("p_action", "DELETE");
                cmd.Parameters.AddWithValue("p_user_id", id);
            });
        }

        public async Task<string?> GetUserRoleNameAsync(int userId, int companyId)
        {
            var result = await ExecuteScalarAsync<object>(@"
                SELECT rm.role_name
                FROM public.tab_user_role ur
                INNER JOIN public.tab_role_master rm ON rm.role_id = ur.role_id
                WHERE ur.user_id = @p_user_id
                  AND ur.company_id = @p_company_id
                ORDER BY rm.role_id
                LIMIT 1",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("p_user_id", userId);
                    cmd.Parameters.AddWithValue("p_company_id", companyId);
                },
                CommandType.Text);

            return result?.ToString();
        }
    }
}
