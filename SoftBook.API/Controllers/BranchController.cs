using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _service;

        public BranchController(IBranchService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new BranchDTO
            {
                BranchId = e.BranchId,
                BranchName = e.BranchName,
                Address = e.Address,
                City = e.City,
                State = e.State,
                Country = e.Country,
                Phone = e.Phone,
                IsActive = e.IsActive
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BranchDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new BranchDTO
            {
                BranchId = result.BranchId,
                BranchName = result.BranchName,
                Address = result.Address,
                City = result.City,
                State = result.State,
                Country = result.Country,
                Phone = result.Phone,
                IsActive = result.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(BranchDTO dto)
        {
            var entity = new Branch
            {
                BranchName = dto.BranchName,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                Phone = dto.Phone,
                IsActive = dto.IsActive
            };
            await _service.InsertAsync(entity);
            dto.BranchId = entity.BranchId;
            return CreatedAtAction(nameof(GetById), new { id = entity.BranchId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BranchDTO dto)
        {
            if (id != dto.BranchId) return BadRequest();
            var entity = new Branch
            {
                BranchId = dto.BranchId,
                BranchName = dto.BranchName,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                Phone = dto.Phone,
                IsActive = dto.IsActive
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
