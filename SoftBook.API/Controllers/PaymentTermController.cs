using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTermController : ControllerBase
    {
        private readonly IPaymentTermService _service;

        public PaymentTermController(IPaymentTermService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentTermDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new PaymentTermDTO
            {
                TermId = e.TermId,
                TermName = e.TermName,
                NumberOfDays = e.NumberOfDays
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentTermDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new PaymentTermDTO
            {
                TermId = result.TermId,
                TermName = result.TermName,
                NumberOfDays = result.NumberOfDays
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentTermDTO dto)
        {
            var entity = new PaymentTerm
            {
                TermName = dto.TermName,
                NumberOfDays = dto.NumberOfDays
            };
            await _service.InsertAsync(entity);
            dto.TermId = entity.TermId;
            return CreatedAtAction(nameof(GetById), new { id = entity.TermId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PaymentTermDTO dto)
        {
            if (id != dto.TermId) return BadRequest();
            var entity = new PaymentTerm
            {
                TermId = dto.TermId,
                TermName = dto.TermName,
                NumberOfDays = dto.NumberOfDays
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
