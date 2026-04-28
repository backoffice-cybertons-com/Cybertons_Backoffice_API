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

namespace SoftBook.API.Controllers
{
    [Route("api/deliveryChallan")]
    public class DeliveryChallanController : ControllerBase
    {
        private readonly IDeliveryChallanService _deliveryChallanService;
        private readonly ITenantProvider _tenantProvider;

        public DeliveryChallanController(IDeliveryChallanService deliveryChallanService, ITenantProvider tenantProvider)
        {
            _deliveryChallanService = deliveryChallanService;
            _tenantProvider = tenantProvider;
        }

        private int GetCompanyId()
        {
            return _tenantProvider.CompanyId ?? 1;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryChallan>>> GetAll()
        {
            var companyId = GetCompanyId();
            var results = await _deliveryChallanService.GetAllAsync(companyId);
            return Ok(results);
        }

        [HttpGet("detailed")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAllDetailed()
        {
            var companyId = GetCompanyId();
            var results = await _deliveryChallanService.GetAllDetailedAsync(companyId);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryChallanViewModel>> GetById(int id)
        {
            var companyId = GetCompanyId();
            var result = await _deliveryChallanService.GetByIdAsync(companyId, id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("composite")]
        public async Task<ActionResult<DbResult>> Create([FromBody] DeliveryChallanViewModel viewModel)
        {
            viewModel.Header.CompanyId = GetCompanyId();
            var result = await _deliveryChallanService.InsertCompositeAsync(viewModel);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("composite")]
        public async Task<ActionResult<DbResult>> Update([FromBody] DeliveryChallanViewModel viewModel)
        {
            viewModel.Header.CompanyId = GetCompanyId();
            var result = await _deliveryChallanService.UpdateCompositeAsync(viewModel);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DbResult>> Delete(int id)
        {
            var companyId = GetCompanyId();
            var result = await _deliveryChallanService.DeleteAsync(companyId, id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
