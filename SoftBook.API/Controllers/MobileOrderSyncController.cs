using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Domain.DTOs;

namespace SoftBook.API.Controllers
{
    [Route("api/mobile-orders")]
    [ApiController]
    public class MobileOrderSyncController : ControllerBase
    {
        private readonly IMobileOrderSyncService _syncService;
        private readonly ITenantProvider _tenantProvider;

        public MobileOrderSyncController(
            IMobileOrderSyncService syncService,
            ITenantProvider tenantProvider)
        {
            _syncService = syncService;
            _tenantProvider = tenantProvider;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncPendingOrders()
        {
            var companyId = _tenantProvider.CompanyId ??0;
            if (companyId == 0)
            {
                return BadRequest("Invalid company context.");
            }

            var userId = _tenantProvider.UserId;

            var result = await _syncService.SyncPendingOrdersAsync(companyId, userId);
            
            // Always return 200 so the frontend can display the real error message.
            // result.Success=false means partial or full sync failure — shown as toast.
            return Ok(result);
        }
    }
}

