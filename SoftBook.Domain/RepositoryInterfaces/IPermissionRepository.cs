using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IPermissionRepository
    {
        /// <summary>
        /// Gets all modules (tabs) with current permission status for a specific role.
        /// </summary>
        Task<List<RolePermission>> GetPermissionsByRoleAsync(int roleId);

        /// <summary>
        /// Saves/Updates permissions for a role in bulk.
        /// </summary>
        Task<bool> SaveRolePermissionsAsync(RolePermissionUpdateDTO permissionUpdateDto);

        /// <summary>
        /// Gets all system modules (tabs) for the company.
        /// </summary>
        Task<List<SidebarItem>> GetAllModulesAsync();
    }
}
