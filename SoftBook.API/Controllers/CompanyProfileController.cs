using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.Data;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyProfileController : ControllerBase
    {
        private readonly ICompanyProfileService _service;
        private readonly ITenantProvider _tenantProvider;

        public CompanyProfileController(ICompanyProfileService service, ITenantProvider tenantProvider)
        {
            _service = service;
            _tenantProvider = tenantProvider;
        }

        private int GetCompanyId()
        {
            return _tenantProvider.CompanyId ?? 1;
        }

        [HttpGet]
        public async Task<ActionResult<CompanyProfile>> Get()
        {
            var companyId = GetCompanyId();
            var profile = await _service.GetByCompanyIdAsync(companyId);
            if (profile == null)
            {
                // Return empty profile with current companyId if not found
                return Ok(new CompanyProfile { CompanyId = companyId });
            }
            return Ok(profile);
        }

        [HttpPost]
        public async Task<ActionResult<DbResult>> Upsert([FromBody] CompanyProfile profile)
        {
            profile.CompanyId = GetCompanyId();
            var result = await _service.UpsertAsync(profile);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(IFormFile file, [FromServices] IWebHostEnvironment env)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            var uploadsFolder = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Uploads", "CompanyLogos");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative URL for frontend
            var fileUrl = $"/Uploads/CompanyLogos/{uniqueFileName}";
            return Ok(new { url = fileUrl, message = "Logo uploaded successfully." });
        }
    }
}
