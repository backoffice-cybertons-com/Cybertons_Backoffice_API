using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications([FromQuery] long companyId, [FromQuery] long userId)
        {
            var notifications = await _notificationService.GetUnreadNotifications(companyId, userId);
            return Ok(notifications);
        }

        [HttpPut("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(long id, [FromQuery] long companyId)
        {
            bool success = await _notificationService.MarkAsRead(companyId, id);
            if (success)
            {
                return Ok(new { Message = "Notification marked as read" });
            }
            return NotFound(new { Message = "Notification not found" });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllNotifications([FromQuery] long companyId, [FromQuery] long userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var notifications = await _notificationService.GetAllNotifications(companyId, userId, page, pageSize);
            return Ok(notifications);
        }

        [HttpPut("mark-toast-shown/{id}")]
        public async Task<IActionResult> MarkToastAsShown(long id, [FromQuery] long companyId)
        {
            bool success = await _notificationService.MarkToastAsShown(companyId, id);
            if (success)
            {
                return Ok(new { Message = "Notification toast marked as shown" });
            }
            return NotFound(new { Message = "Notification not found" });
        }
    }
}
