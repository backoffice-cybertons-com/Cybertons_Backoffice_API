using Microsoft.AspNetCore.Mvc;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SoftBook.API.Hubs;

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
    long senderId,
    long receiverId)
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

    }
}