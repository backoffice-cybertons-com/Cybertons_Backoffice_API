using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransporterController : ControllerBase
    {
        private readonly ITransporterService _service;

        public TransporterController(ITransporterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransporterDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new TransporterDTO
            {
                TransporterId = e.TransporterId,
                TransporterName = e.TransporterName,
                GSTNumber = e.GSTNumber,
                ContactPerson = e.ContactPerson,
                Phone = e.Phone,
                Address = e.Address
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransporterDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new TransporterDTO
            {
                TransporterId = result.TransporterId,
                TransporterName = result.TransporterName,
                GSTNumber = result.GSTNumber,
                ContactPerson = result.ContactPerson,
                Phone = result.Phone,
                Address = result.Address
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransporterDTO dto)
        {
            var entity = new Transporter
            {
                TransporterName = dto.TransporterName,
                GSTNumber = dto.GSTNumber,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Address = dto.Address
            };
            await _service.InsertAsync(entity);
            dto.TransporterId = entity.TransporterId;
            return CreatedAtAction(nameof(GetById), new { id = entity.TransporterId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TransporterDTO dto)
        {
            if (id != dto.TransporterId) return BadRequest();
            var entity = new Transporter
            {
                TransporterId = dto.TransporterId,
                TransporterName = dto.TransporterName,
                GSTNumber = dto.GSTNumber,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
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
