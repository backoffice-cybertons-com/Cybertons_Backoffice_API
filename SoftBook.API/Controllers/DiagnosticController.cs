using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.Entities;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;

namespace SoftBook.API.Controllers
{
    [Route("api/diagnostic")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private readonly ISalePurRepository _salePurRepository;
        private readonly IJournalService _journalService;
        private readonly ITenantProvider _tenantProvider;
        private readonly IJournalRepository _journalRepository;

        public DiagnosticController(
            ISalePurRepository salePurRepository, 
            IJournalService journalService, 
            ITenantProvider tenantProvider,
            IJournalRepository journalRepository)
        {
            _salePurRepository = salePurRepository;
            _journalService = journalService;
            _tenantProvider = tenantProvider;
            _journalRepository = journalRepository;
        }

        [HttpGet("resync-journals")]
        public async Task<IActionResult> ResyncJournals(int companyId, string fromDate = "2026-04-17", string? invNo = null)
        {
            try
            {
                if (!DateTime.TryParse(fromDate, out DateTime startDate))
                    return BadRequest("Invalid fromDate format. Use YYYY-MM-DD.");

                // 1. Get all Sales/Purchase headers for the company
                var allHeaders = await _salePurRepository.GetAllAsync(companyId);
                Console.WriteLine($"[Diagnostic] Found {allHeaders.Count()} total headers for company {companyId}");

                var filteredHeaders = allHeaders.Where(h => h.DeleteFlag == false).ToList();
                
                List<SalePur> targetHeaders;

                // Detailed logging for the requested invoice
                if (!string.IsNullOrEmpty(invNo))
                {
                    var match = filteredHeaders.FirstOrDefault(h => string.Equals(h.InvNo, invNo, StringComparison.OrdinalIgnoreCase));
                    if (match == null)
                    {
                        Console.WriteLine($"[Diagnostic] ERROR: Invoice '{invNo}' not found in database for company {companyId}.");
                        return NotFound($"Invoice '{invNo}' not found.");
                    }
                    if (match.InvDate < startDate)
                    {
                        Console.WriteLine($"[Diagnostic] ERROR: Invoice '{invNo}' date ({match.InvDate:yyyy-MM-dd}) is before start date ({startDate:yyyy-MM-dd}).");
                        return BadRequest($"Invoice '{invNo}' date {match.InvDate:yyyy-MM-dd} is before {startDate:yyyy-MM-dd}. Please adjust fromDate.");
                    }
                    targetHeaders = new List<SalePur> { match };
                }
                else
                {
                    targetHeaders = filteredHeaders
                        .Where(h => h.InvDate >= startDate)
                        .OrderBy(h => h.InvDate)
                        .ToList();
                }

                int totalCount = targetHeaders.Count;
                Console.WriteLine($"[Diagnostic] Processing {totalCount} headers...");
                int successCount = 0;
                int skippedCount = 0;
                var errors = new List<string>();

                foreach (var header in targetHeaders)
                {
                    // Check if journals already exist for this VCode (header.SpUniqueCode)
                    var existingJournals = await _journalRepository.GetByGCodeAsync(header.SpUniqueCode.ToString());
                    if (existingJournals != null && existingJournals.Any())
                    {
                        skippedCount++;
                        continue;
                    }

                    // Get lines for this header
                    var lines = await _salePurRepository.GetLineItemsAsync(header.SpUniqueCode);

                    DbResult result;
                    if (string.Equals(header.InvType, "Sales", StringComparison.OrdinalIgnoreCase))
                    {
                        result = await _journalService.GenerateJournalFromSaleAsync(header, lines);
                    }
                    else if (string.Equals(header.InvType, "Purchase", StringComparison.OrdinalIgnoreCase))
                    {
                        result = await _journalService.GenerateJournalFromPurchaseAsync(header, lines);
                    }
                    else
                    {
                        continue;
                    }

                    if (result.Success)
                    {
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"Inv {header.InvNo} (ID: {header.SpUniqueCode}): {result.Error}");
                    }
                }

                return Ok(new
                {
                    TotalProcessed = totalCount,
                    SuccessfullyGenerated = successCount,
                    AlreadyExisted = skippedCount,
                    Failed = errors.Count,
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("check-settings")]
        public async Task<IActionResult> CheckSettings(int companyId)
        {
            try
            {
                Console.WriteLine($"[Diagnostic] CheckSettings requested for companyId={companyId}");
                // Directly fetch ledger settings to see what IDs are mapped
                var settings = await _journalService.GetLedgerSettingsAsync(companyId);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Diagnostic] CheckSettings ERROR: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("apply-sql-fix")]
        public async Task<IActionResult> ApplySqlFix()
        {
            // Since I cannot execute raw SQL directly through the repository yet without a DDL helper,
            // I will provide the user with the exact script to run.
            return Ok("To apply the fix, please copy the content of 'SoftBook.Infrastructure/sp_Master_Universal_Postgres.sql' and execute it in your PostgreSQL database (PgAdmin/DBeaver).");
        }

        [HttpGet("last-journals")]
        public async Task<IActionResult> GetLastJournals(int companyId)
        {
            try
            {
                Console.WriteLine($"[Diagnostic] Verifying last journals for companyId={companyId}");
                return Ok("Please check your console/terminal. If you see '[JournalRepository] SUCCESS', the entry IS saved. If you don't see it, there is a silent failure.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
