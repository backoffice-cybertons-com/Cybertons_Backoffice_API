using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _service;

        public CurrencyController(ICurrencyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new CurrencyDTO
            {
                CurrencyId = e.CurrencyId,
                CurrencyCode = e.CurrencyCode,
                CurrencySymbol = e.CurrencySymbol,
                CurrencyName = e.CurrencyName,
                DecimalPlaces = e.DecimalPlaces,
                Format = e.Format
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new CurrencyDTO
            {
                CurrencyId = result.CurrencyId,
                CurrencyCode = result.CurrencyCode,
                CurrencySymbol = result.CurrencySymbol,
                CurrencyName = result.CurrencyName,
                DecimalPlaces = result.DecimalPlaces,
                Format = result.Format
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CurrencyDTO dto)
        {
            var entity = new Currency
            {
                CurrencyCode = dto.CurrencyCode,
                CurrencySymbol = dto.CurrencySymbol,
                CurrencyName = dto.CurrencyName,
                DecimalPlaces = dto.DecimalPlaces,
                Format = dto.Format
            };
            await _service.InsertAsync(entity);
            dto.CurrencyId = entity.CurrencyId;
            return CreatedAtAction(nameof(GetById), new { id = entity.CurrencyId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CurrencyDTO dto)
        {
            if (id != dto.CurrencyId) return BadRequest();
            var entity = new Currency
            {
                CurrencyId = dto.CurrencyId,
                CurrencyCode = dto.CurrencyCode,
                CurrencySymbol = dto.CurrencySymbol,
                CurrencyName = dto.CurrencyName,
                DecimalPlaces = dto.DecimalPlaces,
                Format = dto.Format
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
