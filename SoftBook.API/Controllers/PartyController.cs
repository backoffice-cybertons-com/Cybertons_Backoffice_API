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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace SoftBook.API.Controllers
{
    [Route("api/party")]
    public class PartyController : BaseMasterController
    {
        private readonly IPartyService _partyService;
        private readonly IWebHostEnvironment _env;

        public PartyController(IMasterRepository repository, IPartyService partyService, IWebHostEnvironment env) 
            : base(repository, "party")
        {
            _partyService = partyService;
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
    }
}
