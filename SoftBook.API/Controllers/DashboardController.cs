using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Interfaces;
using SoftBook.Domain.Helpers;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Threading.Tasks;

namespace SoftBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ISystemLogRepository _systemLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITenantProvider _tenantProvider;

        public DashboardController(
            IDashboardService dashboardService,
            ISystemLogRepository systemLogRepository,
            IUserRepository userRepository,
            ITenantProvider tenantProvider)
        {
            _dashboardService = dashboardService;
            _systemLogRepository = systemLogRepository;
            _userRepository = userRepository;
            _tenantProvider = tenantProvider;
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

        /// <summary>
        /// Admin-only: per-user activity counts for the selected date range.
        /// </summary>
        [HttpGet("system-log-datewise")]
        public async Task<IActionResult> GetSystemLogDatewise(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var companyId = _tenantProvider.CompanyId ?? 0;
            var userId = _tenantProvider.UserId ?? 0;
            if (companyId <= 0 || userId <= 0)
            {
                return BadRequest(new { message = "Company and user context are required." });
            }

            var roleName = await _userRepository.GetUserRoleNameAsync(userId, companyId);
            if (!string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var to = (toDate ?? DashboardDateHelper.GetBusinessToday()).Date;
            var from = (fromDate ?? to).Date;
            if (from > to)
            {
                return BadRequest(new { message = "From date cannot be after to date." });
            }

            try
            {
                var rows = await _systemLogRepository.GetDatewiseSummaryAsync(companyId, from, to);
                return Ok(rows);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load system log summary.", details = ex.Message });
            }
        }
    }
}
