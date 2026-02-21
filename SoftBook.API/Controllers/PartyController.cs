using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.Entities;
using SoftBook.Domain.ViewModels;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.DTOs;

namespace SoftBook.API.Controllers
{
    [Route("api/party")]
    public class PartyController : BaseMasterController
    {
        private readonly IPartyService _partyService;

        public PartyController(IMasterRepository repository, IPartyService partyService) 
            : base(repository, "party")
        {
            _partyService = partyService;
        }

        [HttpPost("composite")]
        public async Task<ActionResult<Party>> CreateComposite(PartyViewModel viewModel)
        {
            var dbResult = await _partyService.InsertCompositeAsync(viewModel);
            if (!dbResult.Success)
            {
                if (dbResult.Code == "23505") return Conflict(new { error = dbResult.Error });
                return BadRequest(new { error = dbResult.Error });
            }
            return CreatedAtAction(nameof(GetById), new { id = viewModel.Party.PartyId }, viewModel.Party);
        }

        [HttpPut("composite")]
        public async Task<IActionResult> UpdateComposite(PartyViewModel viewModel)
        {
             var dbResult = await _partyService.UpdateCompositeAsync(viewModel);
             if (!dbResult.Success)
             {
                 if (dbResult.Code == "23505") return Conflict(new { error = dbResult.Error });
                 return BadRequest(new { error = dbResult.Error });
             }
             return NoContent();
        }

        [HttpGet("composite/{id}")]
        public async Task<ActionResult<PartyViewModel>> GetComposite(int id)
        {
            var viewModel = await _partyService.GetCompositeAsync(id);
            if (viewModel == null) return NotFound();
            return Ok(viewModel);
        }

        [HttpGet("type/{partyType}")]
        public async Task<ActionResult<IEnumerable<Party>>> GetByType(string partyType)
        {
            var parties = await _partyService.GetByPartyTypeAsync(partyType);
            return Ok(parties);
        }
    }
}
