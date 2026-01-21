using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new ProjectDTO
            {
                ProjectId = e.ProjectId,
                ProjectName = e.ProjectName,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Status = e.Status,
                Description = e.Description
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new ProjectDTO
            {
                ProjectId = result.ProjectId,
                ProjectName = result.ProjectName,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                Status = result.Status,
                Description = result.Description
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectDTO dto)
        {
            var entity = new Project
            {
                ProjectName = dto.ProjectName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                Description = dto.Description
            };
            await _service.InsertAsync(entity);
            dto.ProjectId = entity.ProjectId;
            return CreatedAtAction(nameof(GetById), new { id = entity.ProjectId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProjectDTO dto)
        {
            if (id != dto.ProjectId) return BadRequest();
            var entity = new Project
            {
                ProjectId = dto.ProjectId,
                ProjectName = dto.ProjectName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
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
