using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.API.Helpers;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/salesTransaction")]
    public class SalesTransactionController : ControllerBase
    {
        private readonly ISalesTransactionService _salesTransactionService;
        private readonly ITenantProvider _tenantProvider;
        private readonly IAuditLogService _auditLogService;

        public SalesTransactionController(
            ISalesTransactionService salesTransactionService,
            ITenantProvider tenantProvider,
            IAuditLogService auditLogService)
        {
            _salesTransactionService = salesTransactionService;
            _tenantProvider = tenantProvider;
            _auditLogService = auditLogService;
        }

        private int GetCompanyId()
        {
            return _tenantProvider.CompanyId ?? 1; 
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalePur>>> GetAll()
        {
            var companyId = GetCompanyId();
            var results = await _salesTransactionService.GetAllAsync(companyId);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalesTransactionViewModel>> GetById(int id)
        {
            var companyId = GetCompanyId();
            var result = await _salesTransactionService.GetByIdAsync(companyId, id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("composite")]
        public async Task<ActionResult<DbResult>> SaveComposite([FromBody] SalesTransactionViewModel viewModel)
        {
            if (viewModel == null || viewModel.Header == null)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new DbResult { Success = false, Error = $"Invalid request data. ViewModel or Header is null. Errors: {errors}" });
            }

            viewModel.Header.CompanyId = GetCompanyId();
            var userId = _tenantProvider.UserId;
            var isNew = viewModel.Header.SpUniqueCode <= 0;
            var result = await _salesTransactionService.SaveCompositeAsync(viewModel, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var spUniqueCode = viewModel.Header.SpUniqueCode > 0
                ? viewModel.Header.SpUniqueCode
                : TryParseSpUniqueCode(result);
            await SystemLogHelper.LogSalePurActionAsync(
                _auditLogService,
                _tenantProvider,
                User,
                isNew ? "Add" : "Edit",
                spUniqueCode);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DbResult>> Delete(int id)
        {
            var companyId = GetCompanyId();
            var userId = _tenantProvider.UserId;
            var result = await _salesTransactionService.DeleteAsync(companyId, id, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            await SystemLogHelper.LogSalePurActionAsync(
                _auditLogService,
                _tenantProvider,
                User,
                "Delete",
                id);

            return Ok(result);
        }

        private static int? TryParseSpUniqueCode(DbResult result)
        {
            if (result?.Data == null)
            {
                return null;
            }

            if (result.Data is int intId)
            {
                return intId;
            }

            if (int.TryParse(result.Data.ToString(), out int parsed))
            {
                return parsed;
            }

            return null;
        }

        [HttpGet("outstanding/{partyId}")]
        public async Task<IActionResult> GetOutstanding(int partyId)
        {
            var outstanding = await _salesTransactionService.GetPartyOutstandingAsync(GetCompanyId(), partyId);
            return Ok(outstanding);
        }
    }
}
