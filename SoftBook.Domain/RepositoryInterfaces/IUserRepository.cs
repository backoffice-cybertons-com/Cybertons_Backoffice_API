using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> ValidateUserAsync(string username, string password, int companyId);
        Task<List<SidebarItem>> GetSidebarPermissionsAsync(int userId, int companyId);
        Task<User?> InsertUserAsync(string username, string passwordHash, string email, string? fullName = null, string? phone = null, int? roleId = null, int? companyId = null);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UpdatePasswordAsync(string email, string newPasswordHash);
        Task<string?> GetUserRoleNameAsync(int userId, int companyId);
    }
}
