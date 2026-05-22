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
    }
}
