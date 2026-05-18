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

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
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
    }
}
