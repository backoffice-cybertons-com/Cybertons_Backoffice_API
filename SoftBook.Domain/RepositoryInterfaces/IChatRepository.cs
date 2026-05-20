using SoftBook.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IChatRepository
    {
        Task<Message> SendMessageAsync(Message message);

        Task<List<Message>> GetConversationAsync(long senderId, long receiverId);
        Task MarkMessagesAsReadAsync(long senderId, long receiverId);
        Task<List<ChatListModel>> GetChatListAsync(
            long userId,
            long companyId);
    }
}
