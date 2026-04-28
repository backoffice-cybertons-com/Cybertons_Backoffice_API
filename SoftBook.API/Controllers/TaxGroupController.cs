using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/taxgroup")]
    public class TaxGroupController : BaseMasterController
    {
        public TaxGroupController(IMasterRepository repository) : base(repository, "taxgroup") { }
    }
}
