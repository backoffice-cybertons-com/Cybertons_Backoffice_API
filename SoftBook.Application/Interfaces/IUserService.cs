using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task InsertAsync(User entity);
        Task UpdateAsync(User entity);
        Task DeleteAsync(int id);
        Task<LoginResponseDTO?> ValidateUserAsync(string username, string password, int companyId);
        Task<List<SidebarItem>> GetSidebarPermissionsAsync(int userId, int companyId);
        Task<User?> InsertUserAsync(string username, string password, string email, string fullName = null, string phone = null, int? roleId = null, int? companyId = null);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UpdatePasswordAsync(string email, string newPasswordHash);
    }
}
