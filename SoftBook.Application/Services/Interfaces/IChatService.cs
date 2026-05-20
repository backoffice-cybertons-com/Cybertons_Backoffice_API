using SoftBook.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IChatService
    {
        Task<Message> SendMessageAsync(SendMessageDto dto);

        Task<List<Message>> GetConversationAsync(long senderId, long receiverId);
        Task MarkMessagesAsReadAsync(long senderId, long receiverId);
        Task<List<ChatListModel>> GetChatListAsync(long userId, long companyId);

    }
}
