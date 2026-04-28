using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Security.Claims;
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
            // --- COMPANY ID RESOLUTION ---
            string? companyIdClaim = context.User.FindFirst("CompanyId")?.Value 
                                   ?? context.User.FindFirst("TenantId")?.Value;

            // Fallback to Header if not in token
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

            // --- USER ID RESOLUTION ---
            // After clearing DefaultInboundClaimTypeMap in Program.cs, "sub" and "nameid" are standard.
            var userIdClaim = context.User.FindFirst("UserId")?.Value 
                            ?? context.User.FindFirst("id")?.Value 
                            ?? context.User.FindFirst("uid")?.Value 
                            ?? context.User.FindFirst("sub")?.Value 
                            ?? context.User.FindFirst("nameid")?.Value
                            ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? context.User.FindFirst(ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
            {
                tenantProvider.UserId = userId;
            }
            else
            {
                tenantProvider.UserId = null;
                // Diagnostic: Log if authenticated but no ID found
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    Console.WriteLine($"[TenantMiddleware Warning] Authenticated but no UserId claim found. Claims present: {string.Join(", ", context.User.Claims.Select(c => c.Type))}");
                }
            }

            // --- APPLY TENANT CONTEXT ---
            if (!string.IsNullOrEmpty(companyIdClaim) && int.TryParse(companyIdClaim, out int companyId))
            {
                tenantProvider.CompanyId = companyId;

                // Fiscal year selection (set globally from UI modal)
                string? fiscalYearRaw = null;
                if (context.Request.Headers.TryGetValue("X-Fiscal-Year-Id", out var fiscalHeader))
                {
                    fiscalYearRaw = fiscalHeader.ToString();
                }
                else if (context.Request.Cookies.TryGetValue("sb_fiscal_year_id", out var cookieFiscal))
                {
                    fiscalYearRaw = cookieFiscal;
                }

                if (!string.IsNullOrEmpty(fiscalYearRaw) && int.TryParse(fiscalYearRaw, out var fyId))
                {
                    tenantProvider.FiscalYearId = fyId;
                }
                else
                {
                    tenantProvider.FiscalYearId = null;
                }

                // Resolve and inject the connection string for this shard
                var connectionString = await tenantResolutionService.GetConnectionStringAsync(companyId);
                if (!string.IsNullOrEmpty(connectionString))
                {
                    tenantProvider.ConnectionString = connectionString;
                }
                else
                {
                    Console.WriteLine($"[Tenant Diagnostic] WARNING: No connection string found for CompanyId: {companyId}");
                }
            }
            else
            {
                Console.WriteLine("[Tenant Diagnostic] No CompanyId found in token or headers.");
                tenantProvider.CompanyId = null;
            }

            await _next(context);
        }
    }
}
