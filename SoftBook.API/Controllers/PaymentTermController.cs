using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/paymentterm")]
    public class PaymentTermController : BaseMasterController
    {
        public PaymentTermController(IMasterRepository repository) : base(repository, "paymentterm") { }
    }
}
