using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/partytypemaster")]
    public class PartyTypeMasterController : BaseMasterController
    {
        private readonly IPartyTypeService _partyTypeService;
        private readonly IMasterRepository _masterRepository;

        public PartyTypeMasterController(IMasterRepository repository, IPartyTypeService partyTypeService) 
            : base(repository, "partytype") 
        { 
            _partyTypeService = partyTypeService;
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<dynamic>>> GetAll([FromQuery] int? filterId = null)
        {
            int companyId = TenantProvider?.CompanyId ?? 0;
            var result = await _partyTypeService.GetAllActiveAsync(companyId);
            return Ok(result);
        }
    }
}
