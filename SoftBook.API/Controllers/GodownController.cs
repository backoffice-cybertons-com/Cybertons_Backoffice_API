using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/godown")]
    public class GodownController : BaseMasterController
    {
        public GodownController(IMasterRepository repository) : base(repository, "godown") { }
    }
}
