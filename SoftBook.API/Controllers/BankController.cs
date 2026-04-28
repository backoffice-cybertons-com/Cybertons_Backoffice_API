using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/bank")]
    public class BankController : BaseMasterController
    {
        public BankController(IMasterRepository repository) : base(repository, "bank") { }
    }
}
