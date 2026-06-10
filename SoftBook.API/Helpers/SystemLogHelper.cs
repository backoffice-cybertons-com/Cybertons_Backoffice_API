using System.Security.Claims;
using System.Threading.Tasks;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.Services.Interfaces;

namespace SoftBook.API.Helpers
{
    public static class SystemLogHelper
    {
        public const string SalePurModuleName = "tab_sale_pur";

        public static async Task LogSalePurActionAsync(
            IAuditLogService? auditService,
            ITenantProvider? tenantProvider,
            ClaimsPrincipal user,
            string actionType,
            int? spUniqueCode = null)
        {
            if (auditService == null || tenantProvider?.UserId == null || tenantProvider.CompanyId == null)
            {
                return;
            }

            var username = user.FindFirst(ClaimTypes.Name)?.Value
                ?? user.FindFirst("name")?.Value
                ?? "Unknown";

            await auditService.LogActionAsync(
                actionType,
                tenantProvider.UserId.Value,
                username,
                tenantProvider.CompanyId.Value,
                SalePurModuleName,
                spUniqueCode);
        }
    }
}
