using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationModel>> GetUnreadNotifications(long companyId, long userId);
        Task<bool> MarkAsRead(long companyId, long notificationId);
        Task<List<NotificationModel>> GetAllNotifications(long companyId, long userId, int page = 1, int pageSize = 20);
        Task<bool> MarkToastAsShown(long companyId, long notificationId);
        Task CreateOrUpdateChatNotificationAsync(long companyId, long receiverUserId, long senderUserId, long messageId);
        Task MarkChatNotificationsAsReadAsync(long companyId, long receiverUserId, long senderUserId);
        Task CreateAnnouncementNotificationsAsync(long announcementId, string title, string shortMessage);

        /// <summary>
        /// Soft-deletes delivery rows for an announcement (bell, toast, dropdown).
        /// Use when announcement is deactivated or deleted.
        /// </summary>
        Task RevokeAnnouncementNotificationsAsync(long announcementId);

        /// <summary>
        /// Restores soft-deleted rows and creates missing delivery rows when an announcement is reactivated.
        /// </summary>
        Task RestoreAnnouncementNotificationsAsync(long announcementId, string title, string shortMessage);
    }
}
