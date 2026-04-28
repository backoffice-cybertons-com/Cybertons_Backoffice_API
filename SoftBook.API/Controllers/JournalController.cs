using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/journal")]
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly IJournalService _journalService;
        private readonly ITenantProvider _tenantProvider;

        public JournalController(IJournalService journalService, ITenantProvider tenantProvider)
        {
            _journalService = journalService;
            _tenantProvider = tenantProvider;
        }

        private long GetCompanyId()
        {
            return _tenantProvider.CompanyId ?? 1;
        }

        [HttpGet("direct/{vType}")]
        public async Task<IActionResult> GetDirectJournals(string vType = "Expense")
        {
            var results = await _journalService.GetDirectJournalsAsync(GetCompanyId(), vType);
            return Ok(results);
        }

        [HttpGet("direct")]
        public async Task<IActionResult> GetAllDirectJournals()
        {
            var results = await _journalService.GetDirectJournalsAsync(GetCompanyId(), "ALL");
            return Ok(results);
        }

        [HttpPost("direct")]
        public async Task<ActionResult<DbResult>> SaveDirectJournal([FromBody] List<JournalVoucher> entries)
        {
            if (entries == null || !entries.Any())
            {
                return BadRequest(new DbResult { Success = false, Error = "No journal entries provided." });
            }

            string gCode = string.IsNullOrEmpty(entries.First().GCode) ? $"JV-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}" : entries.First().GCode;

            foreach (var entry in entries)
            {
                entry.JvCompCode = (int)GetCompanyId();
                entry.UserCode = (int?)_tenantProvider.UserId;
                entry.CreatedBy = _tenantProvider.UserId?.ToString() ?? "System";
                entry.GCode = gCode;
            }

            var result = await _journalService.SaveDirectJournalAsync(entries, _tenantProvider.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("direct/{gCode}")]
        public async Task<ActionResult<DbResult>> DeleteDirectJournal(string gCode)
        {
            var result = await _journalService.DeleteJournalByGCodeAsync(gCode, _tenantProvider.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
