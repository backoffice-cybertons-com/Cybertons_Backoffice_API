using System.Collections.Generic;
using System.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using SoftBook.Domain.Services.Interfaces;

namespace SoftBook.Infrastructure.Classes
{
    public class CsvAuditLogService : IAuditLogService
    {
        private readonly string _logDirectory;

        public CsvAuditLogService()
        {
            // Log to 'audit_logs' in the API Project Root for easier access during development
            // Moving up from bin/Debug/net8.0/
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../audit_logs");
            
            // Clean up path
            _logDirectory = Path.GetFullPath(_logDirectory);

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public async Task LogActionAsync(string action, int userId, string username, int companyId)
        {
            try 
            {
                // Create a daily log file: audit_log_20260206.csv
                var fileName = $"audit_log_{DateTime.Now:yyyyMMdd}.csv";
                var filePath = Path.Combine(_logDirectory, fileName);
                
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"{timestamp},{action},{userId},{username},{companyId}{Environment.NewLine}";

                // Check if file exists to add header
                if (!File.Exists(filePath))
                {
                    var header = "Timestamp,Action,UserId,Username,CompanyId" + Environment.NewLine;
                    await File.WriteAllTextAsync(filePath, header + logEntry);
                }
                else
                {
                    await File.AppendAllTextAsync(filePath, logEntry);
                }
            }
            catch (Exception ex)
            {
                // Fallback: Write to console if parsing fails, so we don't crash the request
                Console.WriteLine($"[AuditLog Error] Failed to write log: {ex.Message}");
            }
        }
    }
}
