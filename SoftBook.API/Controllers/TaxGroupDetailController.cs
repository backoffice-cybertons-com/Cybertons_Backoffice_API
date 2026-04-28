using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/taxgroupdetail")]
    public class TaxGroupDetailController : BaseMasterController
    {
        public TaxGroupDetailController(IMasterRepository repository) : base(repository, "taxgroupdetail") { }
    }
}
