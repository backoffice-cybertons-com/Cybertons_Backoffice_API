using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _service;

        public WarehouseController(IWarehouseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new WarehouseDTO
            {
                WarehouseId = e.WarehouseId,
                WarehouseName = e.WarehouseName,
                BranchId = e.BranchId,
                Address = e.Address
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new WarehouseDTO
            {
                WarehouseId = result.WarehouseId,
                WarehouseName = result.WarehouseName,
                BranchId = result.BranchId,
                Address = result.Address
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(WarehouseDTO dto)
        {
            var entity = new Warehouse
            {
                WarehouseName = dto.WarehouseName,
                BranchId = dto.BranchId,
                Address = dto.Address
            };
            await _service.InsertAsync(entity);
            dto.WarehouseId = entity.WarehouseId;
            return CreatedAtAction(nameof(GetById), new { id = entity.WarehouseId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, WarehouseDTO dto)
        {
            if (id != dto.WarehouseId) return BadRequest();
            var entity = new Warehouse
            {
                WarehouseId = dto.WarehouseId,
                WarehouseName = dto.WarehouseName,
                BranchId = dto.BranchId,
                Address = dto.Address
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
