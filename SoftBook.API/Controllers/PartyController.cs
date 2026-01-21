using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Application.Services.Interfaces;

namespace SoftBook.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartyController : ControllerBase
    {
        private readonly IPartyService _partyService;

        public PartyController(IPartyService partyService)
        {
            _partyService = partyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Party>>> GetAll()
        {
            var parties = await _partyService.GetAllAsync();
            return Ok(parties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Party>> GetById(int id)
        {
            var party = await _partyService.GetByIdAsync(id);
            if (party == null) return NotFound();
            return Ok(party);
        }

        [HttpPost]
        public async Task<ActionResult<Party>> Create(Party party)
        {
            await _partyService.InsertAsync(party);
            return CreatedAtAction(nameof(GetById), new { id = party.PartyId }, party);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Party party)
        {
            if (id != party.PartyId) return BadRequest();
            await _partyService.UpdateAsync(party);
            return NoContent();
        }

        [HttpPost("composite")]
        public async Task<ActionResult<Party>> CreateComposite(PartyViewModel viewModel)
        {
            await _partyService.InsertCompositeAsync(viewModel);
            return CreatedAtAction(nameof(GetById), new { id = viewModel.Party.PartyId }, viewModel.Party);
        }

        [HttpPut("composite")]
        public async Task<IActionResult> UpdateComposite(PartyViewModel viewModel)
        {
             await _partyService.UpdateCompositeAsync(viewModel);
             return NoContent();
        }

        [HttpGet("composite/{id}")]
        public async Task<ActionResult<PartyViewModel>> GetComposite(int id)
        {
            var viewModel = await _partyService.GetCompositeAsync(id);
            if (viewModel == null) return NotFound();
            return Ok(viewModel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _partyService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("type/{partyType}")]
        public async Task<ActionResult<IEnumerable<Party>>> GetByType(string partyType)
        {
            var parties = await _partyService.GetByPartyTypeAsync(partyType);
            return Ok(parties);
        }
    }
}
