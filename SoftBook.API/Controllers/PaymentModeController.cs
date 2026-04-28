using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/paymentmode")]
    public class PaymentModeController : BaseMasterController
    {
        public PaymentModeController(IMasterRepository repository) : base(repository, "paymentmode") { }
    }
}
