using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using SoftBook.Domain.DTOs;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterController : ControllerBase
    {
        private readonly IMasterRepository _repository;
        private readonly ITenantProvider _tenantProvider;

        public MasterController(IMasterRepository repository, ITenantProvider tenantProvider)
        {
            _repository = repository;
            _tenantProvider = tenantProvider;
        }

        [HttpGet("{masterName}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAll(string masterName, [FromQuery] int? filterId = null)
        {
            var result = await _repository.GetAllAsync(masterName, filterId);
            return Ok(result);
        }

        [HttpGet("{masterName}/{id}")]
        public async Task<ActionResult<dynamic>> GetById(string masterName, int id)
        {
            var result = await _repository.GetByIdAsync(masterName, id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{masterName}")]
        public async Task<IActionResult> Create(string masterName, [FromBody] JsonElement data)
        {
            var result = await _repository.InsertAsync(masterName, data.GetRawText(), _tenantProvider.UserId);
            if (!result.Success)
            {
                if (result.Code == "23505") return Conflict(new { error = result.Error });
                return BadRequest(new { error = result.Error });
            }
            return Ok(result.Data);
        }

        [HttpPut("{masterName}/{id}")]
        public async Task<IActionResult> Update(string masterName, int id, [FromBody] JsonElement data)
        {
            var result = await _repository.UpdateAsync(masterName, id, data.GetRawText(), _tenantProvider.UserId);
            if (!result.Success)
            {
                if (result.Code == "23505") return Conflict(new { error = result.Error });
                return BadRequest(new { error = result.Error });
            }
            return Ok(result.Data);
        }

        [HttpDelete("{masterName}/{id}")]
        public async Task<IActionResult> Delete(string masterName, int id)
        {
            var result = await _repository.DeleteAsync(masterName, id, _tenantProvider.UserId);
            if (!result.Success)
            {
                if (result.Code == "23503") return Conflict(new { error = result.Error });
                return BadRequest(new { error = result.Error });
            }
            return Ok(new { message = $"{masterName} deleted successfully" });
        }
    }
}
