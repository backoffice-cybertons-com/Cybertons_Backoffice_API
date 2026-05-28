using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Classes
{
    public class OutstandingTermConfigService : IOutstandingTermConfigService
    {
        private static readonly Regex HexColorRegex = new Regex("^#([0-9a-fA-F]{6}|[0-9a-fA-F]{3})$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly IOutstandingTermConfigRepository _repository;
        private readonly ITenantProvider _tenantProvider;

        public OutstandingTermConfigService(
            IOutstandingTermConfigRepository repository,
            ITenantProvider tenantProvider)
        {
            _repository = repository;
            _tenantProvider = tenantProvider;
        }

        public async Task<long> AddAsync(CreateOutstandingTermConfigRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var companyId = ResolveCompanyId(request.CompanyId);
            ValidateRule(companyId, request.MinDays, request.MaxDays, request.BgColor);

            return await _repository.AddAsync(request, companyId);
        }

        public async Task UpdateAsync(UpdateOutstandingTermConfigRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.ConfigId <= 0) throw new ArgumentException("config_id is required.", nameof(request.ConfigId));

            var companyId = ResolveCompanyId(request.CompanyId);
            ValidateRule(companyId, request.MinDays, request.MaxDays, request.BgColor);

            var updated = await _repository.UpdateAsync(request, companyId);
            if (!updated)
            {
                throw new ArgumentException("Outstanding term config not found.");
            }
        }

        public async Task DeleteAsync(long configId)
        {
            if (configId <= 0) throw new ArgumentException("config_id is required.", nameof(configId));

            var deleted = await _repository.SoftDeleteAsync(configId);
            if (!deleted)
            {
                throw new ArgumentException("Outstanding term config not found.");
            }
        }

        public Task<List<OutstandingTermConfigModel>> ListAsync(long companyId)
        {
            if (companyId <= 0) throw new ArgumentException("company_id is required.", nameof(companyId));
            return _repository.GetActiveByCompanyAsync(companyId);
        }

        private long ResolveCompanyId(long? requestCompanyId)
        {
            if (requestCompanyId.HasValue && requestCompanyId.Value > 0) return requestCompanyId.Value;
            if (_tenantProvider.CompanyId.HasValue && _tenantProvider.CompanyId.Value > 0) return _tenantProvider.CompanyId.Value;
            throw new ArgumentException("company_id is required.");
        }

        private static void ValidateRule(long companyId, int? minDays, int? maxDays, string? bgColor)
        {
            if (companyId <= 0)
            {
                throw new ArgumentException("company_id is required.");
            }

            if (!minDays.HasValue)
            {
                throw new ArgumentException("min_days is required.");
            }

            if (!maxDays.HasValue)
            {
                throw new ArgumentException("max_days is required.");
            }

            if (minDays.Value > maxDays.Value)
            {
                throw new ArgumentException("min_days must be less than or equal to max_days.");
            }

            if (string.IsNullOrWhiteSpace(bgColor))
            {
                throw new ArgumentException("bg_color is required.");
            }

            if (!HexColorRegex.IsMatch(bgColor.Trim()))
            {
                throw new ArgumentException("bg_color must be a valid hex color (e.g. #FF0000).");
            }
        }
    }
}

