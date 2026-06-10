using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoftBook.Domain.DTOs;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface ISystemLogRepository
    {
        Task InsertAsync(
            int companyId,
            int userId,
            string actionType,
            string? moduleName = null,
            int? recordId = null,
            string? logMessage = null);

        Task<List<SystemLogDatewiseSummaryDto>> GetDatewiseSummaryAsync(
            int companyId,
            DateTime fromDate,
            DateTime toDate);
    }
}
