using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/vouchertype")]
    public class VoucherTypeController : BaseMasterController
    {
        public VoucherTypeController(IMasterRepository repository) : base(repository, "vouchertype") { }
    }
}
