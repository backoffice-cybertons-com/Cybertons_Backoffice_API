using SoftBook.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Application.Services.Interfaces
{
    public enum ChatDeleteResult
    {
        Deleted,
        NotFound,
        NotAllowed
    }

    public interface IChatService
    {
        Task<Message> SendMessageAsync(SendMessageDto dto);

        Task<List<Message>> GetConversationAsync(long senderId, long receiverId);
        Task MarkMessagesAsReadAsync(long senderId, long receiverId);
        Task<ChatDeleteResult> DeleteMessageAsync(long messageId, long currentUserId);
        Task<List<ChatListModel>> GetChatListAsync(long userId, long companyId);
        Task<List<ChatAttachmentDto>>
 GetChatAttachmentsAsync(
     long senderUserId,
     long receiverUserId,
     long companyId);


    }
}
