using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxGroupDetailController : ControllerBase
    {
        private readonly ITaxGroupDetailService _service;

        public TaxGroupDetailController(ITaxGroupDetailService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxGroupDetail>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaxGroupDetail>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("taxgroup/{taxGroupId}")]
        public async Task<ActionResult<IEnumerable<TaxGroupDetail>>> GetByTaxGroupId(int taxGroupId)
        {
             var result = await _service.GetByTaxGroupIdAsync(taxGroupId);
             return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaxGroupDetail entity)
        {
            await _service.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.TaxGroupDetailId }, entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaxGroupDetail entity)
        {
            if (id != entity.TaxGroupDetailId) return BadRequest();
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
