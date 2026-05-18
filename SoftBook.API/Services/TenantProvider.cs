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
        private int? _companyId;
        private int? _fiscalYearId;
        private int? _userId;
        private string? _connectionString;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? CompanyId
        {
            get => _httpContextAccessor.HttpContext?.Items["CompanyId"] as int? ?? _companyId;
            set
            {
                if (_httpContextAccessor.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items["CompanyId"] = value;
                else
                    _companyId = value;
            }
        }

        public int? FiscalYearId
        {
            get => _httpContextAccessor.HttpContext?.Items["FiscalYearId"] as int? ?? _fiscalYearId;
            set
            {
                if (_httpContextAccessor.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items["FiscalYearId"] = value;
                else
                    _fiscalYearId = value;
            }
        }

        public int? UserId
        {
            get => _httpContextAccessor.HttpContext?.Items["UserId"] as int? ?? _userId;
            set
            {
                if (_httpContextAccessor.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items["UserId"] = value;
                else
                    _userId = value;
            }
        }

        public string? ConnectionString
        {
            get => _httpContextAccessor.HttpContext?.Items["ConnectionString"] as string ?? _connectionString;
            set
            {
                if (_httpContextAccessor.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items["ConnectionString"] = value;
                else
                    _connectionString = value;
            }
        }
    }
}
