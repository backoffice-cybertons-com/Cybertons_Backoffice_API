using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/account")]
    public class AccountController : BaseMasterController
    {
        public AccountController(IMasterRepository repository, ITenantProvider tenantProvider) 
            : base(repository, tenantProvider, "account") { }
    }
}
