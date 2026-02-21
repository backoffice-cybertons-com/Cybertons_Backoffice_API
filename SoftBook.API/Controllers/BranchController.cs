using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/branch")]
    public class BranchController : BaseMasterController
    {
        public BranchController(IMasterRepository repository) : base(repository, "branch") { }
    }
}
