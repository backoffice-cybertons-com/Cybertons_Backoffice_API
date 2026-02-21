using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/fiscalyear")]
    public class FiscalYearController : BaseMasterController
    {
        public FiscalYearController(IMasterRepository repository) : base(repository, "fiscalyear") { }
    }
}
