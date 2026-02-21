using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/tcs")]
    public class TCSController : BaseMasterController
    {
        public TCSController(IMasterRepository repository) : base(repository, "tcs") { }
    }
}
