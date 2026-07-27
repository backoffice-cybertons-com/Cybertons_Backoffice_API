using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Application.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.DTOs;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace SoftBook.API.Controllers
{
    [Route("api/party")]
    public class PartyController : BaseMasterController
    {
        private readonly IPartyService _partyService;
        private readonly IPartyRateAdjustmentService _partyRateAdjustmentService;
        private readonly IWebHostEnvironment _env;

        public PartyController(
            IMasterRepository repository,
            IPartyService partyService,
            IPartyRateAdjustmentService partyRateAdjustmentService,
            IWebHostEnvironment env) 
            : base(repository, "party")
        {
            _partyService = partyService;
            _partyRateAdjustmentService = partyRateAdjustmentService;
            _env = env;
        }

        [HttpPost("composite")]
        public async Task<ActionResult<Party>> CreateComposite(PartyViewModel viewModel)
        {
            var userId = TenantProvider?.UserId;
            var dbResult = await _partyService.InsertCompositeAsync(viewModel, userId);
            if (!dbResult.Success)
            {
                if (dbResult.Code == "23505") return Conflict(new { error = dbResult.Error });
                return BadRequest(new { error = dbResult.Error });
            }
            return CreatedAtAction(nameof(GetById), new { id = viewModel.Party.PartyId }, viewModel.Party);
        }

        [HttpPut("composite")]
        public async Task<IActionResult> UpdateComposite(PartyViewModel viewModel)
        {
             var userId = TenantProvider?.UserId;
             var dbResult = await _partyService.UpdateCompositeAsync(viewModel, userId);
             if (!dbResult.Success)
             {
                 if (dbResult.Code == "23505") return Conflict(new { error = dbResult.Error });
                 return BadRequest(new { error = dbResult.Error });
             }
             return NoContent();
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(int id)
        {
            var userId = TenantProvider?.UserId;
            var dbResult = await _partyService.DeleteAsync(id, userId);
            if (!dbResult.Success)
            {
                return BadRequest(new { error = dbResult.Error });
            }
            return Ok(new { message = $"Party deleted successfully" });
        }

        [HttpGet("composite/{id}")]
        public async Task<ActionResult<PartyViewModel>> GetComposite(int id)
        {
            var viewModel = await _partyService.GetCompositeAsync(id);
            if (viewModel == null) return NotFound();
            return Ok(viewModel);
        }

        [HttpGet("type/{partyType}")]
        public async Task<ActionResult<IEnumerable<Party>>> GetByType(string partyType)
        {
            var parties = await _partyService.GetByPartyTypeAsync(partyType);
            return Ok(parties);
        }

        [HttpGet("rate-adjustment-summary")]
        public async Task<ActionResult<IEnumerable<PartyRateAdjustmentSummaryItemDto>>> GetRateAdjustmentSummary(
            [FromQuery] string? partyType = "Customer,Trader",
            [FromQuery] bool onlyWithAddPrice = true)
        {
            var companyId = TenantProvider?.CompanyId ?? 0;
            if (companyId <= 0)
            {
                return BadRequest(new { error = "Company context not found." });
            }

            var result = await _partyRateAdjustmentService.GetSummaryListAsync(companyId, partyType, onlyWithAddPrice);
            return Ok(result);
        }

        [HttpGet("rate-adjustment-changes")]
        public async Task<ActionResult<IEnumerable<PartyRateAdjustmentChangeItemDto>>> GetRateAdjustmentChanges(
            [FromQuery] string? partyType = "Customer,Trader",
            [FromQuery] string? changeType = null)
        {
            var companyId = TenantProvider?.CompanyId ?? 0;
            if (companyId <= 0)
            {
                return BadRequest(new { error = "Company context not found." });
            }

            var result = await _partyRateAdjustmentService.GetChangeListAsync(companyId, partyType, changeType);
            return Ok(result);
        }

        [HttpGet("rate-adjustment-invoice-mismatches")]
        public async Task<ActionResult<IEnumerable<PartyInvoiceAddPriceMismatchDto>>> GetRateAdjustmentInvoiceMismatches(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? partyType = "Customer,Trader")
        {
            var companyId = TenantProvider?.CompanyId ?? 0;
            if (companyId <= 0)
            {
                return BadRequest(new { error = "Company context not found." });
            }

            var to = (toDate ?? DateTime.Today).Date;
            var from = (fromDate ?? to.AddDays(-30)).Date;
            if (from > to)
            {
                return BadRequest(new { error = "From date cannot be after to date." });
            }

            var result = await _partyRateAdjustmentService.GetInvoiceMismatchListAsync(companyId, from, to, partyType);
            return Ok(result);
        }

        /// <summary>
        /// Virtual add-price calculation only (does not update tab_party_master).
        /// </summary>
        [HttpGet("{partyId}/calculate-rate-adjustment")]
        public async Task<ActionResult<PartyRateAdjustmentResultDto>> CalculateRateAdjustment(int partyId)
        {
            var companyId = TenantProvider?.CompanyId ?? 0;
            if (companyId <= 0)
            {
                return BadRequest(new { error = "Company context not found." });
            }

            var result = await _partyRateAdjustmentService.CalculateAsync(companyId, partyId);
            return Ok(result);
        }

        /// <summary>
        /// Calculates add price and persists to tab_party_master only when AutoPersistRateDiff is enabled.
        /// </summary>
        [HttpPost("{partyId}/recalculate-rate-adjustment")]
        public async Task<IActionResult> RecalculateRateAdjustment(int partyId)
        {
            var companyId = TenantProvider?.CompanyId ?? 0;
            if (companyId <= 0)
            {
                return BadRequest(new { error = "Company context not found." });
            }

            var result = await _partyRateAdjustmentService.CalculateAndUpdateAsync(companyId, partyId);
            return Ok(result);
        }

        [HttpPost("upload-profile")]
        public async Task<IActionResult> UploadProfile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "PartyProfiles");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var relativePath = "/Uploads/PartyProfiles/" + uniqueFileName;
            return Ok(new { filePath = relativePath });
        }

        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Save to SoftBook.API/Upload directory
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Upload");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var safeFileName = Path.GetFileName(file.FileName).Replace(" ", "_");
            var uniqueFileName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + safeFileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path
            var relativePath = "Upload/" + uniqueFileName;
            return Ok(new { filePath = relativePath });
        }
    }
}
