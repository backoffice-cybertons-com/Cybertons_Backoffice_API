using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxController : ControllerBase
    {
        private readonly ITaxService _service;

        public TaxController(ITaxService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new TaxDTO
            {
                TaxId = e.TaxId,
                TaxName = e.TaxName,
                RatePercent = e.RatePercent,
                TaxType = e.TaxType
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaxDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new TaxDTO
            {
                TaxId = result.TaxId,
                TaxName = result.TaxName,
                RatePercent = result.RatePercent,
                TaxType = result.TaxType
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaxDTO dto)
        {
            var entity = new Tax
            {
                TaxName = dto.TaxName,
                RatePercent = dto.RatePercent,
                TaxType = dto.TaxType
            };
            await _service.InsertAsync(entity);
            dto.TaxId = entity.TaxId;
            return CreatedAtAction(nameof(GetById), new { id = entity.TaxId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaxDTO dto)
        {
            if (id != dto.TaxId) return BadRequest();
            var entity = new Tax
            {
                TaxId = dto.TaxId,
                TaxName = dto.TaxName,
                RatePercent = dto.RatePercent,
                TaxType = dto.TaxType
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
