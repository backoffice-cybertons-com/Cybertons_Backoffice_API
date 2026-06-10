using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SoftBook.Domain.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task LogActionAsync(
            string action,
            int userId,
            string username,
            int companyId,
            string? moduleName = null,
            int? recordId = null);
    }
}
