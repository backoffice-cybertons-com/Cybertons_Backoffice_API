using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SACController : ControllerBase
    {
        private readonly ISACService _service;

        public SACController(ISACService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SACDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new SACDTO
            {
                SACId = e.SACId,
                SACCode = e.SACCode,
                Description = e.Description,
                TaxRate = e.TaxRate
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SACDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new SACDTO
            {
                SACId = result.SACId,
                SACCode = result.SACCode,
                Description = result.Description,
                TaxRate = result.TaxRate
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SACDTO dto)
        {
            var entity = new SAC
            {
                SACCode = dto.SACCode,
                Description = dto.Description,
                TaxRate = dto.TaxRate
            };
            await _service.InsertAsync(entity);
            dto.SACId = entity.SACId;
            return CreatedAtAction(nameof(GetById), new { id = entity.SACId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SACDTO dto)
        {
            if (id != dto.SACId) return BadRequest();
            var entity = new SAC
            {
                SACId = dto.SACId,
                SACCode = dto.SACCode,
                Description = dto.Description,
                TaxRate = dto.TaxRate
            };
            await _service.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
