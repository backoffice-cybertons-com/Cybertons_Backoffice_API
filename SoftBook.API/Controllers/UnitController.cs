using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _service;

        public UnitController(IUnitService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new UnitDTO
            {
                UnitId = e.UnitId,
                UnitName = e.UnitName,
                UQC = e.UQC
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new UnitDTO
            {
                UnitId = result.UnitId,
                UnitName = result.UnitName,
                UQC = result.UQC
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UnitDTO dto)
        {
            var entity = new Unit
            {
                UnitName = dto.UnitName,
                UQC = dto.UQC
            };
            await _service.InsertAsync(entity);
            dto.UnitId = entity.UnitId;
            return CreatedAtAction(nameof(GetById), new { id = entity.UnitId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UnitDTO dto)
        {
            if (id != dto.UnitId) return BadRequest();
            var entity = new Unit
            {
                UnitId = dto.UnitId,
                UnitName = dto.UnitName,
                UQC = dto.UQC
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
