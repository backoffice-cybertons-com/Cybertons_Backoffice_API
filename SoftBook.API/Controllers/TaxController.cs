using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/tax")]
    public class TaxController : BaseMasterController
    {
        public TaxController(IMasterRepository repository) : base(repository, "tax") { }
    }
}
