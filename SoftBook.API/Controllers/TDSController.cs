using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/tds")]
    public class TDSController : BaseMasterController
    {
        public TDSController(IMasterRepository repository) : base(repository, "tds") { }
    }
}
