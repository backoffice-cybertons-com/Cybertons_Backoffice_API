using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Task<List<NotificationModel>> GetUnreadNotifications(long companyId, long userId);
        Task<bool> MarkAsRead(long companyId, long notificationId);
        Task<List<NotificationModel>> GetAllNotifications(long companyId, long userId, int page = 1, int pageSize = 20);
        Task<long> AddNotification(NotificationModel notification);
        Task<bool> MarkToastAsShown(long companyId, long notificationId);
        Task<NotificationModel?> GetUnreadChatNotificationAsync(long companyId, long receiverUserId, long senderUserId);
        Task UpdateChatNotificationAsync(long notificationId, string message, long referenceId, int notificationCount);
        Task<string?> GetUserDisplayNameAsync(long userId);
        Task MarkChatNotificationsAsReadAsync(long companyId, long receiverUserId, long senderUserId);
    }
}
