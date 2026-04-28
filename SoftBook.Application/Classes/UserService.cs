using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.Data;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;

namespace SoftBook.Application.Services.Classes
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task InsertAsync(User entity)
        {
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(User entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<LoginResponseDTO?> ValidateUserAsync(string username, string password, int companyId)
        {
            var user = await _repository.ValidateUserAsync(username, password, companyId);
            if (user == null) return null;
            return new LoginResponseDTO { User = user };
        }

        public async Task<List<SidebarItem>> GetSidebarPermissionsAsync(int userId, int companyId)
        {
            return await _repository.GetSidebarPermissionsAsync(userId, companyId);
        }

        public async Task<User?> InsertUserAsync(string username, string passwordHash, string email, string fullName = null, string phone = null, int? roleId = null, int? companyId = null)
        {
            return await _repository.InsertUserAsync(username, passwordHash, email, fullName, phone, roleId, companyId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _repository.GetUserByEmailAsync(email);
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPasswordHash)
        {
            return await _repository.UpdatePasswordAsync(email, newPasswordHash);
        }

    }
}
