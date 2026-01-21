using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentModeController : ControllerBase
    {
        private readonly IPaymentModeService _service;

        public PaymentModeController(IPaymentModeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentModeDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new PaymentModeDTO
            {
                PaymentModeId = e.PaymentModeId,
                ModeName = e.ModeName,
                IsDefault = e.IsDefault
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentModeDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new PaymentModeDTO
            {
                PaymentModeId = result.PaymentModeId,
                ModeName = result.ModeName,
                IsDefault = result.IsDefault
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentModeDTO dto)
        {
            var entity = new PaymentMode
            {
                ModeName = dto.ModeName,
                IsDefault = dto.IsDefault
            };
            await _service.InsertAsync(entity);
            dto.PaymentModeId = entity.PaymentModeId;
            return CreatedAtAction(nameof(GetById), new { id = entity.PaymentModeId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PaymentModeDTO dto)
        {
            if (id != dto.PaymentModeId) return BadRequest();
            var entity = new PaymentMode
            {
                PaymentModeId = dto.PaymentModeId,
                ModeName = dto.ModeName,
                IsDefault = dto.IsDefault
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
