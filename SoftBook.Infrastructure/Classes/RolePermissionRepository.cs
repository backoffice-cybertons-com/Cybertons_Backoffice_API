using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SoftBook.Domain.Data;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.Entities;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.Infrastructure.Repositories.Classes
{
    public class RolePermissionRepository : BaseDAL, IPermissionRepository
    {
        public RolePermissionRepository(IConfiguration configuration, ITenantProvider tenantProvider) 
            : base(configuration, tenantProvider) { }

        public async Task<List<RolePermission>> GetPermissionsByRoleAsync(int roleId)
        {
            // Join tab_role_permission and tabs to get name and url
            string query = @"
                SELECT 
                    p.permission_id, p.company_id, p.role_id, p.tab_id, 
                    p.can_view, p.can_add, p.can_edit, p.can_delete, p.is_active,
                    t.tab_name, t.tab_url
                FROM tab_role_permission p
                JOIN tabs t ON p.tab_id = t.tab_id
                WHERE p.company_id = @company_id AND p.role_id = @role_id;";

            return await ExecuteReaderAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("company_id", _tenantProvider.CompanyId);
                cmd.Parameters.AddWithValue("role_id", roleId);
            }, reader => new RolePermission
            {
                PermissionId = SafeGetInt(reader, "permission_id"),
                CompanyId = SafeGetInt(reader, "company_id"),
                RoleId = SafeGetInt(reader, "role_id"),
                TabId = SafeGetInt(reader, "tab_id"),
                CanView = SafeGetBool(reader, "can_view") ?? false,
                CanAdd = SafeGetBool(reader, "can_add") ?? false,
                CanEdit = SafeGetBool(reader, "can_edit") ?? false,
                CanDelete = SafeGetBool(reader, "can_delete") ?? false,
                IsActive = SafeGetBool(reader, "is_active") ?? true,
                TabName = SafeGetString(reader, "tab_name"),
                TabUrl = SafeGetString(reader, "tab_url")
            }, CommandType.Text);
        }

        public async Task<List<SidebarItem>> GetAllModulesAsync()
        {
            // For management UI, we want ALL modules (tabs). 
            // Note: The tabs table does not have a company_id column as per schema analysis.
            string query = @"
                SELECT tab_id, tab_name, tab_url, parent_id, icon_path, sort_order 
                FROM tabs 
                ORDER BY sort_order;";

            return await ExecuteReaderAsync(query, cmd => { }, reader => new SidebarItem
            {
                TabId = SafeGetInt(reader, "tab_id"),
                TabName = SafeGetString(reader, "tab_name"),
                TabUrl = SafeGetString(reader, "tab_url"),
                ParentId = reader["parent_id"] != DBNull.Value ? Convert.ToInt32(reader["parent_id"]) : (int?)null,
                IconPath = SafeGetString(reader, "icon_path"),
                SortOrder = SafeGetInt(reader, "sort_order")
            }, CommandType.Text);
        }

        public async Task<bool> SaveRolePermissionsAsync(RolePermissionUpdateDTO permissionUpdateDto)
        {
            // We use our JSONB bulk save function for atomicity
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            string jsonPermissions = JsonSerializer.Serialize(permissionUpdateDto.Permissions, jsonOptions);
            
            await ExecuteNonQueryAsync("fn_role_permission_bulk_save", cmd =>
            {
                cmd.Parameters.AddWithValue("p_company_id", _tenantProvider.CompanyId);
                cmd.Parameters.AddWithValue("p_role_id", permissionUpdateDto.RoleId);
                cmd.Parameters.AddWithValue("p_permissions", NpgsqlTypes.NpgsqlDbType.Jsonb, jsonPermissions);
            });

            return true;
        }
    }
}
