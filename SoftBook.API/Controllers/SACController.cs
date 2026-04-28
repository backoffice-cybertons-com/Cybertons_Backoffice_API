using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/sac")]
    public class SACController : BaseMasterController
    {
        public SACController(IMasterRepository repository) : base(repository, "sac") { }
    }
}
