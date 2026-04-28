using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SoftBook.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace SoftBook.API.Services
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? CompanyId
        {
            get => _httpContextAccessor.HttpContext?.Items["CompanyId"] as int?;
            set
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["CompanyId"] = value;
                }
            }
        }

        public int? FiscalYearId
        {
            get => _httpContextAccessor.HttpContext?.Items["FiscalYearId"] as int?;
            set
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["FiscalYearId"] = value;
                }
            }
        }

        public int? UserId
        {
            get => _httpContextAccessor.HttpContext?.Items["UserId"] as int?;
            set
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["UserId"] = value;
                }
            }
        }

        public string? ConnectionString
        {
            get => _httpContextAccessor.HttpContext?.Items["ConnectionString"] as string;
            set
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Items["ConnectionString"] = value;
                }
            }
        }
    }
}
