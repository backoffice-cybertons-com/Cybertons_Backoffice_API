using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/salesperson")]
    public class SalespersonController : BaseMasterController
    {
        public SalespersonController(IMasterRepository repository) : base(repository, "salesperson") { }
    }
}
