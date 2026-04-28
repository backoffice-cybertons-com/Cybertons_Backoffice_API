using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/currency")]
    public class CurrencyController : BaseMasterController
    {
        public CurrencyController(IMasterRepository repository) : base(repository, "currency") { }
    }
}
