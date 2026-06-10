using System;
using System.Threading.Tasks;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.Services.Interfaces;

namespace SoftBook.Infrastructure.Classes
{
    public class DbAuditLogService : IAuditLogService
    {
        private readonly ISystemLogRepository _systemLogRepository;

        public DbAuditLogService(ISystemLogRepository systemLogRepository)
        {
            _systemLogRepository = systemLogRepository;
        }

        public async Task LogActionAsync(
            string action,
            int userId,
            string username,
            int companyId,
            string? moduleName = null,
            int? recordId = null)
        {
            try
            {
                await _systemLogRepository.InsertAsync(
                    companyId,
                    userId,
                    action,
                    moduleName,
                    recordId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SystemLog Error] Failed to write log: {ex.Message}");
            }
        }
    }
}
