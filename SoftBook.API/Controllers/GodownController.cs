using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GodownController : ControllerBase
    {
        private readonly IGodownService _service;

        public GodownController(IGodownService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GodownDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new GodownDTO
            {
                GodownId = e.GodownId,
                WarehouseId = e.WarehouseId,
                GodownName = e.GodownName
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GodownDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new GodownDTO
            {
                GodownId = result.GodownId,
                WarehouseId = result.WarehouseId,
                GodownName = result.GodownName
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(GodownDTO dto)
        {
            var entity = new Godown
            {
                WarehouseId = dto.WarehouseId,
                GodownName = dto.GodownName
            };
            await _service.InsertAsync(entity);
            dto.GodownId = entity.GodownId;
            return CreatedAtAction(nameof(GetById), new { id = entity.GodownId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GodownDTO dto)
        {
            if (id != dto.GodownId) return BadRequest();
            var entity = new Godown
            {
                GodownId = dto.GodownId,
                WarehouseId = dto.WarehouseId,
                GodownName = dto.GodownName
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
