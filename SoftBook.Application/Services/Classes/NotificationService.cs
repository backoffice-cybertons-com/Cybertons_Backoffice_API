using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Classes
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationRealtimeNotifier _realtimeNotifier;

        public NotificationService(
            INotificationRepository notificationRepository,
            INotificationRealtimeNotifier realtimeNotifier)
        {
            _notificationRepository = notificationRepository;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<List<NotificationModel>> GetUnreadNotifications(long companyId, long userId)
        {
            return await _notificationRepository.GetUnreadNotifications(companyId, userId);
        }

        public async Task<bool> MarkAsRead(long companyId, long notificationId)
        {
            return await _notificationRepository.MarkAsRead(companyId, notificationId);
        }

        public async Task<List<NotificationModel>> GetAllNotifications(long companyId, long userId, int page = 1, int pageSize = 20)
        {
            return await _notificationRepository.GetAllNotifications(companyId, userId, page, pageSize);
        }

        public async Task<bool> MarkToastAsShown(long companyId, long notificationId)
        {
            return await _notificationRepository.MarkToastAsShown(companyId, notificationId);
        }

        public async Task CreateOrUpdateChatNotificationAsync(
            long companyId,
            long receiverUserId,
            long senderUserId,
            long messageId)
        {
            if (receiverUserId == senderUserId)
            {
                return;
            }

            var senderName = await _notificationRepository.GetUserDisplayNameAsync(senderUserId);
            if (string.IsNullOrWhiteSpace(senderName))
            {
                senderName = "Someone";
            }

            var existing = await _notificationRepository.GetUnreadChatNotificationAsync(
                companyId,
                receiverUserId,
                senderUserId);

            if (existing == null)
            {
                var notification = new NotificationModel
                {
                    CompanyId = companyId,
                    UserId = receiverUserId,
                    Title = "New Message",
                    Message = BuildChatNotificationMessage(senderName, 1),
                    ReferenceType = "CHAT",
                    ReferenceId = messageId,
                    NotificationCount = 1,
                    SenderUserId = senderUserId
                };

                await _notificationRepository.AddNotification(notification);
                await _realtimeNotifier.NotifyUserAsync(receiverUserId);
                return;
            }

            var newCount = existing.NotificationCount + 1;
            var message = BuildChatNotificationMessage(senderName, newCount);

            await _notificationRepository.UpdateChatNotificationAsync(
                existing.NotificationId,
                message,
                messageId,
                newCount);

            await _realtimeNotifier.NotifyUserAsync(receiverUserId);
        }

        public async Task MarkChatNotificationsAsReadAsync(
            long companyId,
            long receiverUserId,
            long senderUserId)
        {
            await _notificationRepository.MarkChatNotificationsAsReadAsync(
                companyId,
                receiverUserId,
                senderUserId);
        }

        internal static string BuildChatNotificationMessage(string senderName, int count)
        {
            return count > 1
                ? $"{count} new messages from {senderName}"
                : $"{senderName} sent you a message";
        }
    }
}
