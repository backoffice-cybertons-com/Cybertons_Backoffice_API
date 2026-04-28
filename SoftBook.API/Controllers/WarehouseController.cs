using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/warehouse")]
    public class WarehouseController : BaseMasterController
    {
        public WarehouseController(IMasterRepository repository) : base(repository, "warehouse") { }
    }
}
