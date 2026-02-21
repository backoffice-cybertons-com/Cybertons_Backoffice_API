using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/project")]
    public class ProjectController : BaseMasterController
    {
        public ProjectController(IMasterRepository repository) : base(repository, "project") { }
    }
}
