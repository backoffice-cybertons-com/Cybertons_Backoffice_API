using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FiscalYearController : ControllerBase
    {
        private readonly IFiscalYearService _service;

        public FiscalYearController(IFiscalYearService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FiscalYearDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new FiscalYearDTO
            {
                FiscalYearId = e.FiscalYearId,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrent = e.IsCurrent
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FiscalYearDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new FiscalYearDTO
            {
                FiscalYearId = result.FiscalYearId,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                IsCurrent = result.IsCurrent
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(FiscalYearDTO dto)
        {
            var entity = new FiscalYear
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsCurrent = dto.IsCurrent
            };
            await _service.InsertAsync(entity);
            dto.FiscalYearId = entity.FiscalYearId;
            return CreatedAtAction(nameof(GetById), new { id = entity.FiscalYearId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, FiscalYearDTO dto)
        {
            if (id != dto.FiscalYearId) return BadRequest();
            var entity = new FiscalYear
            {
                FiscalYearId = dto.FiscalYearId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsCurrent = dto.IsCurrent
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
