using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/role")]
    public class RoleController : BaseMasterController
    {
        public RoleController(IMasterRepository repository) : base(repository, "role") { }
    }
}
