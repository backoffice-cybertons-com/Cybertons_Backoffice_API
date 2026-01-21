using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> ValidateUserAsync(string username, string password, int companyId);
        Task<List<SidebarItem>> GetSidebarPermissionsAsync(int userId);
        Task<User?> InsertUserAsync(string username, string passwordHash, string email, string fullName = null, string phone = null);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UpdatePasswordAsync(string email, string newPasswordHash);
    }
}
