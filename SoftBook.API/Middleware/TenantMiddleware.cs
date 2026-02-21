using System.Security.Claims;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider, ITenantResolutionService tenantResolutionService)
        {
            // 1. Try to get CompanyId from JWT Claims
            var companyIdClaim = context.User.FindFirst("CompanyId")?.Value;
            
            // Fallback for TenantId (legacy support)
            if (string.IsNullOrEmpty(companyIdClaim))
            {
                companyIdClaim = context.User.FindFirst("TenantId")?.Value;
            }

            // Fallback to Header (dev/test only)
            if (string.IsNullOrEmpty(companyIdClaim))
            {
                 if (context.Request.Headers.TryGetValue("X-Company-Id", out var id))
                 {
                     companyIdClaim = id.ToString();
                 }
                 else if (context.Request.Headers.TryGetValue("X-Tenant-Id", out id))
                 {
                     companyIdClaim = id.ToString();
                 }
            }

            if (!string.IsNullOrEmpty(companyIdClaim) && int.TryParse(companyIdClaim, out int companyId))
            {
                tenantProvider.CompanyId = companyId;
                Console.WriteLine($"[Tenant Diagnostic] Resolved CompanyId: {companyId}");

                // Resolve and inject the connection string for this shard
                var connectionString = await tenantResolutionService.GetConnectionStringAsync(companyId);
                if (!string.IsNullOrEmpty(connectionString))
                {
                    tenantProvider.ConnectionString = connectionString;
                    Console.WriteLine($"[Tenant Diagnostic] Switched to DB: {connectionString}");
                }
                else
                {
                    Console.WriteLine($"[Tenant Diagnostic] WARNING: No connection string found for CompanyId: {companyId}");
                }
            }
            else
            {
                Console.WriteLine("[Tenant Diagnostic] No CompanyId claim found in request.");
            }

            await _next(context);
        }
    }
}
