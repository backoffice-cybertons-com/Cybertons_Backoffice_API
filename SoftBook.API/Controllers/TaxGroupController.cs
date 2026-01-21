using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.DTOs;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxGroupController : ControllerBase
    {
        private readonly ITaxGroupService _service;

        public TaxGroupController(ITaxGroupService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxGroupDTO>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result.Select(e => new TaxGroupDTO
            {
                TaxGroupId = e.TaxGroupId,
                GroupName = e.GroupName,
                Description = e.Description,
                SupplyType = e.SupplyType
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaxGroupDTO>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new TaxGroupDTO
            {
                TaxGroupId = result.TaxGroupId,
                GroupName = result.GroupName,
                Description = result.Description,
                SupplyType = result.SupplyType
            });
        }

        [HttpGet("supplytype/{supplyType}")]
        public async Task<ActionResult<IEnumerable<TaxGroupWithRateDTO>>> GetBySupplyTypeWithRates(string supplyType)
        {
             var result = await _service.GetBySupplyTypeWithRatesAsync(supplyType);
             return Ok(result.Select(e => new TaxGroupWithRateDTO
             {
                 TaxGroupId = e.TaxGroupId,
                 GroupName = e.GroupName,
                 SupplyType = e.SupplyType,
                 TotalRatePercent = e.TotalRatePercent
             }));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaxGroupDTO dto)
        {
            var entity = new TaxGroup
            {
                GroupName = dto.GroupName,
                Description = dto.Description,
                SupplyType = dto.SupplyType
            };
            await _service.InsertAsync(entity);
            dto.TaxGroupId = entity.TaxGroupId;
            return CreatedAtAction(nameof(GetById), new { id = entity.TaxGroupId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaxGroupDTO dto)
        {
            if (id != dto.TaxGroupId) return BadRequest();
            var entity = new TaxGroup
            {
                TaxGroupId = dto.TaxGroupId,
                GroupName = dto.GroupName,
                Description = dto.Description,
                SupplyType = dto.SupplyType
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
