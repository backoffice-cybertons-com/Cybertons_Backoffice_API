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

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
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

                CreatedBy = dto.SenderUserId
            };

            return await _chatRepository.SendMessageAsync(message);
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
        }

        public async Task<List<ChatListModel>> GetChatListAsync(
    long userId,
    long companyId)
        {
            return await _chatRepository.GetChatListAsync(
                userId,
                companyId);
        }
    }
}