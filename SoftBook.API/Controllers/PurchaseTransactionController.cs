using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.Services.Interfaces;
using SoftBook.API.Helpers;

namespace SoftBook.API.Controllers
{
    [Route("api/purchaseTransaction")]
    public class PurchaseTransactionController : ControllerBase
    {
        private readonly IPurchaseTransactionService _purchaseTransactionService;
        private readonly ITenantProvider _tenantProvider;
        private readonly IAuditLogService _auditLogService;

        public PurchaseTransactionController(
            IPurchaseTransactionService purchaseTransactionService,
            ITenantProvider tenantProvider,
            IAuditLogService auditLogService)
        {
            _purchaseTransactionService = purchaseTransactionService;
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
            var results = await _purchaseTransactionService.GetAllAsync(companyId);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseTransactionViewModel>> GetById(int id)
        {
            var companyId = GetCompanyId();
            var result = await _purchaseTransactionService.GetByIdAsync(companyId, id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("composite")]
        public async Task<ActionResult<DbResult>> SaveComposite([FromBody] PurchaseTransactionViewModel viewModel)
        {
            if (viewModel == null || viewModel.Header == null)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new DbResult { Success = false, Error = $"Invalid request data. ViewModel or Header is null. Errors: {errors}" });
            }

            viewModel.Header.CompanyId = GetCompanyId();
            var isNew = viewModel.Header.SpUniqueCode <= 0;
            var result = await _purchaseTransactionService.SaveCompositeAsync(viewModel);
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
            var result = await _purchaseTransactionService.DeleteAsync(companyId, id);
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
    }
}
