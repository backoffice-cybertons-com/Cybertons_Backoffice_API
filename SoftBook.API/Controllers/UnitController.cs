using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/unit")]
    public class UnitController : BaseMasterController
    {
        public UnitController(IMasterRepository repository) : base(repository, "unit") { }
    }
}
