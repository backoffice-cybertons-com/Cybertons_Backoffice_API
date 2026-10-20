using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.DTOs.GstIrn;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.Data;

namespace SoftBook.API.Controllers;

[Route("api/gst-irn")]
[ApiController]
public class GstIrnController : ControllerBase
{
    private readonly IGstIrnService _gstIrnService;
    private readonly ITenantProvider _tenantProvider;

    public GstIrnController(IGstIrnService gstIrnService, ITenantProvider tenantProvider)
    {
        _gstIrnService = gstIrnService;
        _tenantProvider = tenantProvider;
    }

    private int GetCompanyId() => _tenantProvider.CompanyId ?? 1;

    [HttpGet("settings")]
    public async Task<ActionResult<GstIrnSettingsDto>> GetSettings()
    {
        var settings = await _gstIrnService.GetSettingsAsync(GetCompanyId());
        return Ok(settings);
    }

    [HttpPost("settings")]
    public async Task<ActionResult<GstIrnSettingsDto>> SaveSettings([FromBody] GstIrnSettingsDto settings)
    {
        await _gstIrnService.SaveSettingsAsync(GetCompanyId(), settings);
        return Ok(settings);
    }

    [HttpGet("{spUniqueCode}/json")]
    public async Task<IActionResult> GetPayloadJson(int spUniqueCode)
    {
        try
        {
            var json = await _gstIrnService.BuildPayloadJsonAsync(GetCompanyId(), spUniqueCode);
            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{spUniqueCode}")]
    public async Task<ActionResult<GstIrnStatusDto>> GetStatus(int spUniqueCode)
    {
        var status = await _gstIrnService.GetStatusAsync(GetCompanyId(), spUniqueCode);
        if (status == null)
        {
            return NotFound(new { message = "No IRN record found for this invoice." });
        }

        return Ok(status);
    }

    [HttpPost("{spUniqueCode}/generate")]
    public async Task<ActionResult<GstIrnGenerateResultDto>> Generate(int spUniqueCode)
    {
        try
        {
            var result = await _gstIrnService.GenerateAsync(GetCompanyId(), spUniqueCode, _tenantProvider.UserId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new GstIrnGenerateResultDto
            {
                Success = false,
                Message = ex.Message,
                SpUniqueCode = spUniqueCode
            });
        }
    }
}
