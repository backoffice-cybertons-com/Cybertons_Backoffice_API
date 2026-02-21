using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using SoftBook.Domain.DTOs;

namespace SoftBook.API.Controllers
{
    [ApiController]
    public abstract class BaseMasterController : ControllerBase
    {
        protected readonly IMasterRepository _repository;
        protected readonly string _masterName;

        protected BaseMasterController(IMasterRepository repository, string masterName)
        {
            _repository = repository;
            _masterName = masterName;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<dynamic>>> GetAll([FromQuery] int? filterId = null)
        {
            var result = await _repository.GetAllAsync(_masterName, filterId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<dynamic>> GetById(int id)
        {
            var result = await _repository.GetByIdAsync(_masterName, id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] JsonElement data)
        {
            var dbResult = await _repository.InsertAsync(_masterName, data.GetRawText());
            if (!dbResult.Success)
            {
                if (dbResult.Code == "23505") return Conflict(new { error = dbResult.Error });
                return BadRequest(new { error = dbResult.Error });
            }
            return Ok(dbResult.Data);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, [FromBody] JsonElement data)
        {
            var dbResult = await _repository.UpdateAsync(_masterName, id, data.GetRawText());
            if (!dbResult.Success)
            {
                if (dbResult.Code == "23505") return Conflict(new { error = dbResult.Error });
                return BadRequest(new { error = dbResult.Error });
            }
            return Ok(dbResult.Data);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var dbResult = await _repository.DeleteAsync(_masterName, id);
            if (!dbResult.Success)
            {
                 return BadRequest(new { error = dbResult.Error });
            }
            return Ok(new { message = $"{_masterName} deleted successfully" });
        }
    }
}
