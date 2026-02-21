using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/transporter")]
    public class TransporterController : BaseMasterController
    {
        public TransporterController(IMasterRepository repository) : base(repository, "transporter") { }
    }
}
