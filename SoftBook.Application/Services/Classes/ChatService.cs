using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SoftBook.Application.Services.Classes
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly INotificationService _notificationService;

        public ChatService(
            IChatRepository chatRepository,
            INotificationService notificationService)
        {
            _chatRepository = chatRepository;
            _notificationService = notificationService;
        }

        public async Task<Message> SendMessageAsync(SendMessageDto dto)
        {
            Message message = new Message
            {
                CompanyId = dto.CompanyId,

                SenderUserId = dto.SenderUserId,

                ReceiverUserId = dto.ReceiverUserId,

                MessageText = dto.MessageText,

                MessageType = dto.MessageType,

                ReplyMessageId = dto.ReplyMessageId,

                CreatedBy = dto.SenderUserId,
                FileLink = dto.FileLink,
            };

            var saved = await _chatRepository.SendMessageAsync(message);

            await _notificationService.CreateOrUpdateChatNotificationAsync(
                dto.CompanyId,
                dto.ReceiverUserId,
                dto.SenderUserId,
                saved.MessageId);

            return saved;
        }

        public async Task<List<Message>> GetConversationAsync(
            long senderId,
            long receiverId)
        {
            return await _chatRepository.GetConversationAsync(
                senderId,
                receiverId);
        }

        public async Task MarkMessagesAsReadAsync(
    long senderId,
    long receiverId)
        {
            await _chatRepository.MarkMessagesAsReadAsync(
                senderId,
                receiverId);

            var companyId = await _chatRepository.GetCompanyIdForConversationAsync(senderId, receiverId);
            if (companyId.HasValue)
            {
                await _notificationService.MarkChatNotificationsAsReadAsync(
                    companyId.Value,
                    receiverId,
                    senderId);
            }
        }

        public async Task<ChatDeleteResult> DeleteMessageAsync(long messageId, long currentUserId)
        {
            var message = await _chatRepository.GetMessageByIdAsync(messageId);

            if (message == null)
            {
                return ChatDeleteResult.NotFound;
            }

            if (message.SenderUserId != currentUserId)
            {
                return ChatDeleteResult.NotAllowed;
            }

            var deleted = await _chatRepository.DeleteMessageAsync(messageId, currentUserId);

            return deleted ? ChatDeleteResult.Deleted : ChatDeleteResult.NotFound;
        }

        public async Task<List<ChatListModel>> GetChatListAsync(
    long userId,
    long companyId)
        {
            return await _chatRepository.GetChatListAsync(
                userId,
                companyId);
        }

        public async Task<List<ChatAttachmentDto>>GetChatAttachmentsAsync(
    long senderUserId,
    long receiverUserId,
    long companyId)
        {
            return await _chatRepository.GetChatAttachmentsAsync(senderUserId, receiverUserId, companyId);
        }
    }
}
