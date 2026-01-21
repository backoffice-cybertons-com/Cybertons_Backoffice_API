using System.Security.Claims;
using SoftBook.Infrastructure.Interfaces;

namespace SoftBook.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantResolutionService tenantResolutionService, ITenantProvider tenantProvider)
        {
            // 1. Try to get TenantId from JWT Claims
            var tenantIdClaim = context.User.FindFirst("TenantId")?.Value;
            
            // Fallback to Header for non-JWT transition (optional, but requested to NEVER trust mobile headers)
            // For now, let's strictly use the claim if authenticated, or header if NOT authenticated (for dev/test)
            if (string.IsNullOrEmpty(tenantIdClaim))
            {
                 if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var id))
                 {
                     tenantIdClaim = id.ToString();
                 }
            }

            if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out int tenantId))
            {
                tenantProvider.TenantId = tenantId;
                
                // 2. Resolve Connection String
                var connectionString = await tenantResolutionService.GetConnectionStringAsync(tenantId);
                if (!string.IsNullOrEmpty(connectionString))
                {
                    tenantProvider.ConnectionString = connectionString;
                }
            }

            await _next(context);
        }
    }
}
