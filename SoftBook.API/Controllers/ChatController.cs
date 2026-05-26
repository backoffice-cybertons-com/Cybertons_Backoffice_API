using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SoftBook.API.Hubs;
using SoftBook.API.Models;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SoftBook.API.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(
       IChatService chatService,
       IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;

            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(
      [FromBody] SendMessageDto dto)
        {
            try
            {
                var result = await _chatService.SendMessageAsync(dto);

                await _hubContext.Clients
                    .All
                    .SendAsync("ReceiveMessage", result);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("conversation/{senderId}/{receiverId}")]
        public async Task<IActionResult> GetConversation(
            long senderId,
            long receiverId)
        {
            try
            {
                var result = await _chatService.GetConversationAsync(
                    senderId,
                    receiverId);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("mark-as-read")]
        public async Task<IActionResult> MarkAsRead(
            [FromQuery] long senderId,
            [FromQuery] long receiverId)
        {
            try
            {
                await _chatService.MarkMessagesAsReadAsync(
                    senderId,
                    receiverId);

                return Ok(new
                {
                    success = true,
                    message = "Messages marked as read"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet("chat-list/{userId}/{companyId}")]
        public async Task<IActionResult> GetChatList(
    long userId,
    long companyId)
        {
            try
            {
                var result = await _chatService.GetChatListAsync(
                    userId,
                    companyId);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        //[AllowAnonymous]
        [HttpDelete("message/{messageId}")]
        public async Task<IActionResult> DeleteMessage(
            long messageId,
            [FromQuery] long? senderUserId = null)
        {
            try
            {
                // Same pattern as send/mark-as-read: client supplies user id.
                // JWT claim is used when present (optional auth on this controller).
                var currentUserId = GetCurrentUserId() ?? senderUserId;

                if (!currentUserId.HasValue || currentUserId.Value <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "senderUserId is required to delete a message"
                    });
                }

                var result = await _chatService.DeleteMessageAsync(messageId, currentUserId.Value);

                if (result == ChatDeleteResult.NotFound)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Message not found or already deleted"
                    });
                }

                if (result == ChatDeleteResult.NotAllowed)
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Only the sender can delete this message"
                    });
                }

                await _hubContext.Clients
                    .All
                    .SendAsync("MessageDeleted", messageId);

                return Ok(new
                {
                    success = true,
                    messageId,
                    message = "Message deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private long? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value
                ?? User.FindFirst("user_id")?.Value
                ?? User.FindFirst("id")?.Value
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst("nameid")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(ClaimTypes.Name)?.Value;

            return long.TryParse(userIdClaim, out var userId) ? userId : null;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadAttachment(
    [FromForm] SendAttachmentRequest dto)
        {
            try
            {
                if (dto.File == null || dto.File.Length == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No file selected."
                    });
                }

                // 2MB validation
                if (dto.File.Length > 2 * 1024 * 1024)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "File size cannot exceed 2MB."
                    });
                }

                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "chat-attachments");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string extension = Path.GetExtension(dto.File.FileName);

                string fileName =
                    $"{Guid.NewGuid()}{extension}";

                string filePath = Path.Combine(
                    uploadsFolder,
                    fileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                string fileUrl =
                    $"/chat-attachments/{fileName}";

                SendMessageDto messageDto = new SendMessageDto
                {
                    CompanyId = dto.CompanyId,

                    SenderUserId = dto.SenderUserId,

                    ReceiverUserId = dto.ReceiverUserId,

                    MessageText = dto.File.FileName,

                    MessageType = "file",

                    FileLink = fileUrl
                };

                var result =
                    await _chatService.SendMessageAsync(messageDto);

                await _hubContext.Clients
                    .All
                    .SendAsync("ReceiveMessage", result);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet("attachments")]
        public async Task<IActionResult> GetChatAttachments(
     long senderUserId,
     long receiverUserId,
     long companyId)
        {
            try
            {
                var result =
                    await _chatService.GetChatAttachmentsAsync(
                        senderUserId,
                        receiverUserId,
                        companyId);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

    }
}
