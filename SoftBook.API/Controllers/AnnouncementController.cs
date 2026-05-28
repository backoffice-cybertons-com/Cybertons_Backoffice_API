using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Threading.Tasks;

namespace SoftBook.API.Controllers
{
    [Route("api/announcements")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;
        private readonly ITenantProvider _tenantProvider;

        public AnnouncementController(
            IAnnouncementService announcementService,
            ITenantProvider tenantProvider)
        {
            _announcementService = announcementService;
            _tenantProvider = tenantProvider;
        }

        /// <summary>
        /// Admin endpoint to create a new announcement (hidden route).
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAnnouncementRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Invalid request body." });
            }

            try
            {
                var announcementId = await _announcementService.CreateAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Announcement created successfully.",
                    announcementId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Admin: list all announcements for management UI.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var announcements = await _announcementService.GetAllAsync();
                return Ok(new { success = true, data = announcements });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Admin: update an existing announcement.
        /// </summary>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateAnnouncementRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Invalid request body." });
            }

            try
            {
                await _announcementService.UpdateAsync(id, request);
                return Ok(new { success = true, message = "Announcement updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Admin: soft-delete (deactivate) an announcement and revoke delivery notifications.
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _announcementService.DeleteAsync(id);
                return Ok(new { success = true, message = "Announcement deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Admin: activate or deactivate an announcement.
        /// </summary>
        [HttpPatch("{id:long}/active")]
        public async Task<IActionResult> SetActive(long id, [FromBody] SetAnnouncementActiveRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Invalid request body." });
            }

            try
            {
                await _announcementService.SetActiveAsync(id, request.IsActive);
                return Ok(new
                {
                    success = true,
                    message = request.IsActive
                        ? "Announcement activated successfully."
                        : "Announcement deactivated successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Returns full announcement details by id (for drawer/modal display).
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            if (id <= 0)
            {
                return BadRequest(new { success = false, message = "A valid announcement id is required." });
            }

            try
            {
                var announcement = await _announcementService.GetByIdAsync(id);

                if (announcement == null)
                {
                    return NotFound(new { success = false, message = "Announcement not found." });
                }

                return Ok(new { success = true, data = announcement });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Returns the latest active, in-window announcement not yet seen by the current user.
        /// </summary>
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest([FromQuery] long? userId = null)
        {
            var resolvedUserId = ResolveUserId(userId);
            if (!resolvedUserId.HasValue)
            {
                return BadRequest(new { success = false, message = "userId is required." });
            }

            try
            {
                var announcement = await _announcementService.GetLatestUnseenAsync(resolvedUserId.Value);

                if (announcement == null)
                {
                    return Ok(new { success = true, data = (AnnouncementModel?)null });
                }

                return Ok(new { success = true, data = announcement });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Marks an announcement as seen for the current user so it is not shown again.
        /// </summary>
        [HttpPost("seen")]
        public async Task<IActionResult> MarkSeen(
            [FromBody] MarkAnnouncementSeenRequest request,
            [FromQuery] long? userId = null)
        {
            if (request == null || request.AnnouncementId <= 0)
            {
                return BadRequest(new { success = false, message = "announcement_id is required." });
            }

            var resolvedUserId = ResolveUserId(userId);
            if (!resolvedUserId.HasValue)
            {
                return BadRequest(new { success = false, message = "userId is required." });
            }

            try
            {
                await _announcementService.MarkAsSeenAsync(request.AnnouncementId, resolvedUserId.Value);

                return Ok(new
                {
                    success = true,
                    message = "Announcement marked as seen."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private long? ResolveUserId(long? queryUserId)
        {
            if (_tenantProvider.UserId.HasValue && _tenantProvider.UserId.Value > 0)
            {
                return _tenantProvider.UserId.Value;
            }

            if (queryUserId.HasValue && queryUserId.Value > 0)
            {
                return queryUserId.Value;
            }

            return null;
        }
    }
}
