using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/account")]
    public class AccountController : BaseMasterController
    {
        public AccountController(IMasterRepository repository) : base(repository, "account") { }
    }
}
