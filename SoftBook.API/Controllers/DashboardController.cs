using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Interfaces;
using SoftBook.Domain.DTOs;
using System.Threading.Tasks;

namespace SoftBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpPost]
        public async Task<IActionResult> GetDashboardData([FromBody] DashboardRequestDto request)
        {
            if (request.CompanyId <= 0)
            {
                return BadRequest(new { message = "CompanyId is required." });
            }

            try
            {
                var resultJson = await _dashboardService.GetDashboardDataAsync(request);
                return Content(resultJson, "application/json");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching dashboard data.", details = ex.Message });
            }
        }
    }
}
