using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartyTypeMasterController : ControllerBase
    {
        private readonly IPartyTypeMasterService _service;

        public PartyTypeMasterController(IPartyTypeMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartyTypeMaster>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PartyTypeMaster>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("type/{partyType}")]
        public async Task<ActionResult<IEnumerable<Party>>> GetPartiesByType(string partyType)
        {
            var result = await _service.GetPartiesByTypeAsync(partyType);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartyTypeMaster entity)
        {
            await _service.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.PartyTypeId }, entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PartyTypeMaster entity)
        {
            if (id != entity.PartyTypeId) return BadRequest();
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
