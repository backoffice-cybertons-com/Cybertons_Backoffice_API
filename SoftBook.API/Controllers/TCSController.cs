using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TCSController : ControllerBase
    {
        private readonly ITCSService _service;

        public TCSController(ITCSService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TCSDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new TCSDTO
            {
                TCSId = e.TCSId,
                Rate = e.Rate,
                TaxName = e.TaxName,
                NatureOfCollection = e.NatureOfCollection,
                IsHigherRate = e.IsHigherRate,
                StartDate = e.StartDate,
                EndDate = e.EndDate
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TCSDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new TCSDTO
            {
                TCSId = result.TCSId,
                Rate = result.Rate,
                TaxName = result.TaxName,
                NatureOfCollection = result.NatureOfCollection,
                IsHigherRate = result.IsHigherRate,
                StartDate = result.StartDate,
                EndDate = result.EndDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(TCSDTO dto)
        {
            var entity = new TCS
            {
                Rate = dto.Rate,
                TaxName = dto.TaxName,
                NatureOfCollection = dto.NatureOfCollection,
                IsHigherRate = dto.IsHigherRate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            await _service.InsertAsync(entity);
            dto.TCSId = entity.TCSId;
            return CreatedAtAction(nameof(GetById), new { id = entity.TCSId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TCSDTO dto)
        {
            if (id != dto.TCSId) return BadRequest();
            var entity = new TCS
            {
                TCSId = dto.TCSId,
                Rate = dto.Rate,
                TaxName = dto.TaxName,
                NatureOfCollection = dto.NatureOfCollection,
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
