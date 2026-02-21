using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/partytypemaster")]
    public class PartyTypeMasterController : BaseMasterController
    {
        public PartyTypeMasterController(IMasterRepository repository) : base(repository, "partytype") { }
    }
}
