using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/costcenter")]
    public class CostCenterController : BaseMasterController
    {
        public CostCenterController(IMasterRepository repository) : base(repository, "costcenter") { }
    }
}
