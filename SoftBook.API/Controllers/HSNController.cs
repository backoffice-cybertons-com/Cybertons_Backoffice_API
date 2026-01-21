using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HSNController : ControllerBase
    {
        private readonly IHSNService _service;

        public HSNController(IHSNService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HSNDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new HSNDTO
            {
                HSNId = e.HSNId,
                HSNCode = e.HSNCode,
                Description = e.Description,
                TaxRate = e.TaxRate
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HSNDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new HSNDTO
            {
                HSNId = result.HSNId,
                HSNCode = result.HSNCode,
                Description = result.Description,
                TaxRate = result.TaxRate
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(HSNDTO dto)
        {
            var entity = new HSN
            {
                HSNCode = dto.HSNCode,
                Description = dto.Description,
                TaxRate = dto.TaxRate
            };
            await _service.InsertAsync(entity);
            dto.HSNId = entity.HSNId;
            return CreatedAtAction(nameof(GetById), new { id = entity.HSNId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, HSNDTO dto)
        {
            if (id != dto.HSNId) return BadRequest();
            var entity = new HSN
            {
                HSNId = dto.HSNId,
                HSNCode = dto.HSNCode,
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
