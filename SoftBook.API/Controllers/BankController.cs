using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly IBankService _service;

        public BankController(IBankService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new BankDTO
            {
                BankMasterId = e.BankMasterId,
                BankName = e.BankName,
                Branch = e.Branch,
                AccountNumber = e.AccountNumber,
                IFSC = e.IFSC,
                MICR = e.MICR
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BankDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new BankDTO
            {
                BankMasterId = result.BankMasterId,
                BankName = result.BankName,
                Branch = result.Branch,
                AccountNumber = result.AccountNumber,
                IFSC = result.IFSC,
                MICR = result.MICR
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(BankDTO dto)
        {
            var entity = new Bank
            {
                BankName = dto.BankName,
                Branch = dto.Branch,
                AccountNumber = dto.AccountNumber,
                IFSC = dto.IFSC,
                MICR = dto.MICR
            };
            await _service.InsertAsync(entity);
            dto.BankMasterId = entity.BankMasterId;
            return CreatedAtAction(nameof(GetById), new { id = entity.BankMasterId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BankDTO dto)
        {
            if (id != dto.BankMasterId) return BadRequest();
            var entity = new Bank
            {
                BankMasterId = dto.BankMasterId,
                BankName = dto.BankName,
                Branch = dto.Branch,
                AccountNumber = dto.AccountNumber,
                IFSC = dto.IFSC,
                MICR = dto.MICR
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
