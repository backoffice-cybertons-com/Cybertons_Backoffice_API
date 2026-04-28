using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/ledgersetting")]
    public class LedgerSettingController : BaseMasterController
    {
        public LedgerSettingController(IMasterRepository repository) : base(repository, "ledgersetting") { }
    }
}
