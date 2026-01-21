using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CostCenterController : ControllerBase
    {
        private readonly ICostCenterService _service;

        public CostCenterController(ICostCenterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CostCenterDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new CostCenterDTO
            {
                CostCenterId = e.CostCenterId,
                CostCenterName = e.CostCenterName,
                Description = e.Description
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CostCenterDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new CostCenterDTO
            {
                CostCenterId = result.CostCenterId,
                CostCenterName = result.CostCenterName,
                Description = result.Description
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CostCenterDTO dto)
        {
            var entity = new CostCenter
            {
                CostCenterName = dto.CostCenterName,
                Description = dto.Description
            };
            await _service.InsertAsync(entity);
            dto.CostCenterId = entity.CostCenterId;
            return CreatedAtAction(nameof(GetById), new { id = entity.CostCenterId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CostCenterDTO dto)
        {
            if (id != dto.CostCenterId) return BadRequest();
            var entity = new CostCenter
            {
                CostCenterId = dto.CostCenterId,
                CostCenterName = dto.CostCenterName,
                Description = dto.Description
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
