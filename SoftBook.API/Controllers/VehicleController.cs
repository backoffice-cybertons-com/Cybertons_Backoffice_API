using System;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Domain.RepositoryInterfaces;

namespace SoftBook.API.Controllers
{
    [Route("api/vehicle")]
    public class VehicleController : BaseMasterController
    {
        public VehicleController(IMasterRepository repository) : base(repository, "vehicle")
        {
        }
    }
}
