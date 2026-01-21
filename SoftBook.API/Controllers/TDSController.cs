using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TDSController : ControllerBase
    {
        private readonly ITDSService _service;

        public TDSController(ITDSService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TDSDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new TDSDTO
            {
                TDSId = e.TDSId,
                TaxName = e.TaxName,
                Rate = e.Rate,
                Section = e.Section,
                IsHigherRate = e.IsHigherRate,
                StartDate = e.StartDate,
                EndDate = e.EndDate
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TDSDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new TDSDTO
            {
                TDSId = result.TDSId,
                TaxName = result.TaxName,
                Rate = result.Rate,
                Section = result.Section,
                IsHigherRate = result.IsHigherRate,
                StartDate = result.StartDate,
                EndDate = result.EndDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(TDSDTO dto)
        {
            var entity = new TDS
            {
                TaxName = dto.TaxName,
                Rate = dto.Rate,
                Section = dto.Section,
                IsHigherRate = dto.IsHigherRate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            await _service.InsertAsync(entity);
            dto.TDSId = entity.TDSId;
            return CreatedAtAction(nameof(GetById), new { id = entity.TDSId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TDSDTO dto)
        {
            if (id != dto.TDSId) return BadRequest();
            var entity = new TDS
            {
                TDSId = dto.TDSId,
                TaxName = dto.TaxName,
                Rate = dto.Rate,
                Section = dto.Section,
                IsHigherRate = dto.IsHigherRate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
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
