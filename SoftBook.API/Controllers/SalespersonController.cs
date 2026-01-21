using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalespersonController : ControllerBase
    {
        private readonly ISalespersonService _service;

        public SalespersonController(ISalespersonService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalespersonDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new SalespersonDTO
            {
                SalespersonId = e.SalespersonId,
                SalespersonName = e.SalespersonName,
                Email = e.Email
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalespersonDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new SalespersonDTO
            {
                SalespersonId = result.SalespersonId,
                SalespersonName = result.SalespersonName,
                Email = result.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SalespersonDTO dto)
        {
            var entity = new Salesperson
            {
                SalespersonName = dto.SalespersonName,
                Email = dto.Email
            };
            await _service.InsertAsync(entity);
            dto.SalespersonId = entity.SalespersonId;
            return CreatedAtAction(nameof(GetById), new { id = entity.SalespersonId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SalespersonDTO dto)
        {
            if (id != dto.SalespersonId) return BadRequest();
            var entity = new Salesperson
            {
                SalespersonId = dto.SalespersonId,
                SalespersonName = dto.SalespersonName,
                Email = dto.Email
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
