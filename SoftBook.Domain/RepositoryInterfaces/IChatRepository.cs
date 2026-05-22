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
        Task<Message?> GetMessageByIdAsync(long messageId);
        Task<bool> DeleteMessageAsync(long messageId, long senderUserId);
        Task<List<ChatListModel>> GetChatListAsync(
            long userId,
            long companyId);
        Task<List<ChatAttachmentDto>>GetChatAttachmentsAsync( long senderUserId, long receiverUserId, long companyId);
        Task<long?> GetCompanyIdForConversationAsync(long userId1, long userId2);

    }
}
