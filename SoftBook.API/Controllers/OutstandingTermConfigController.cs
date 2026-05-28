using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using System;
using System.Threading.Tasks;

namespace SoftBook.API.Controllers
{
    [Route("api/outstanding-term-config")]
    [ApiController]
    public class OutstandingTermConfigController : ControllerBase
    {
        private readonly IOutstandingTermConfigService _service;

        public OutstandingTermConfigController(IOutstandingTermConfigService service)
        {
            _service = service;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CreateOutstandingTermConfigRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Invalid request body." });
            }

            try
            {
                var configId = await _service.AddAsync(request);
                return Ok(new { success = true, message = "Rule added successfully.", configId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateOutstandingTermConfigRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Invalid request body." });
            }

            try
            {
                await _service.UpdateAsync(request);
                return Ok(new { success = true, message = "Rule updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete/{configId:long}")]
        public async Task<IActionResult> Delete(long configId)
        {
            try
            {
                await _service.DeleteAsync(configId);
                return Ok(new { success = true, message = "Rule deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("list/{companyId:long}")]
        public async Task<IActionResult> List(long companyId)
        {
            try
            {
                var rules = await _service.ListAsync(companyId);
                return Ok(new { success = true, data = rules });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}

