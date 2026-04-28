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
    [Route("api/paymentReceipt")]
    public class PaymentReceiptController : ControllerBase
    {
        private readonly IPayRecService _payRecService;
        private readonly ITenantProvider _tenantProvider;

        public PaymentReceiptController(IPayRecService payRecService, ITenantProvider tenantProvider)
        {
            _payRecService = payRecService;
            _tenantProvider = tenantProvider;
        }

        private long GetCompanyId()
        {
            return _tenantProvider.CompanyId ?? 1;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList(int pageNo = 1, int pageSize = 50, string? vType = null, int? partyId = null)
        {
            var results = await _payRecService.GetPaymentListAsync(GetCompanyId(), pageNo, pageSize, vType, partyId);
            return Ok(results);
        }

        [HttpGet("pending/{partyId}")]
        public async Task<IActionResult> GetPendingInvoices(int partyId, string trnType)
        {
            // trnType should be 'Purchase' for Payments and 'Sales' for Receipts usually,
            // but the UI chooses based on the action.
            var results = await _payRecService.GetPendingInvoicesAsync(trnType, partyId);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<PayRec>>> GetById(long id)
        {
            var result = await _payRecService.GetByIdAsync(GetCompanyId(), id);
            if (result == null || !result.Any()) return NotFound();
            return Ok(result);
        }

        [HttpPost("composite")]
        public async Task<ActionResult<DbResult>> SaveComposite([FromBody] List<PayRec> entities)
        {
            if (entities == null || !entities.Any())
            {
                return BadRequest(new DbResult { Success = false, Error = "No data provided." });
            }

            foreach (var entity in entities)
            {
                entity.CompanyId = GetCompanyId();
                entity.UserCode = (int?)_tenantProvider.UserId;
                entity.CreatedBy = _tenantProvider.UserId?.ToString() ?? "System";
            }

            var userId = _tenantProvider.UserId;
            var result = await _payRecService.SaveCompositeAsync(entities, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DbResult>> Delete(long id)
        {
            var userId = _tenantProvider.UserId;
            var result = await _payRecService.DeleteAsync(GetCompanyId(), id, _tenantProvider.UserId?.ToString() ?? "System", userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
