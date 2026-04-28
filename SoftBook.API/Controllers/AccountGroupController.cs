using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/accountgroup")]
    public class AccountGroupController : BaseMasterController
    {
        public AccountGroupController(IMasterRepository repository, ITenantProvider tenantProvider) 
            : base(repository, tenantProvider, "accountgroup") { }
    }
}
