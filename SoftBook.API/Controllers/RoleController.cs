using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new RoleDTO
            {
                RoleId = e.RoleId,
                RoleName = e.RoleName
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new RoleDTO
            {
                RoleId = result.RoleId,
                RoleName = result.RoleName
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleDTO dto)
        {
            var entity = new Role
            {
                RoleName = dto.RoleName
            };
            await _service.InsertAsync(entity);
            dto.RoleId = entity.RoleId;
            return CreatedAtAction(nameof(GetById), new { id = entity.RoleId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RoleDTO dto)
        {
            if (id != dto.RoleId) return BadRequest();
            var entity = new Role
            {
                RoleId = dto.RoleId,
                RoleName = dto.RoleName
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
