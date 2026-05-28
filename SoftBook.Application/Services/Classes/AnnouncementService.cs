using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Announcement lifecycle: tab_announcements = source of truth; tab_notifications = delivery/UI only.
namespace SoftBook.Application.Services.Classes
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly INotificationService _notificationService;

        public AnnouncementService(
            IAnnouncementRepository announcementRepository,
            INotificationService notificationService)
        {
            _announcementRepository = announcementRepository;
            _notificationService = notificationService;
        }

        public async Task<long> CreateAsync(CreateAnnouncementRequest request)
        {
            ValidateRequest(request);

            var announcementId = await _announcementRepository.CreateAsync(request);

            if (request.IsActive)
            {
                await DeliverAnnouncementNotificationsAsync(announcementId, request);
            }

            return announcementId;
        }

        public Task<List<AnnouncementModel>> GetAllAsync() =>
            _announcementRepository.GetAllAsync();

        public async Task UpdateAsync(long announcementId, UpdateAnnouncementRequest request)
        {
            if (announcementId <= 0)
            {
                throw new ArgumentException("A valid announcement id is required.", nameof(announcementId));
            }

            ValidateRequest(request);

            var existing = await _announcementRepository.GetByIdAsync(announcementId);
            if (existing == null)
            {
                throw new ArgumentException("Announcement not found.");
            }

            var wasActive = existing.IsActive;
            var updated = await _announcementRepository.UpdateAsync(announcementId, request);
            if (!updated)
            {
                throw new ArgumentException("Announcement not found.");
            }

            await SyncNotificationDeliveryAsync(
                announcementId,
                wasActive,
                request.IsActive,
                request.Title!,
                request.ShortMessage ?? request.Title!);
        }

        public async Task DeleteAsync(long announcementId)
        {
            if (announcementId <= 0)
            {
                throw new ArgumentException("A valid announcement id is required.", nameof(announcementId));
            }

            var existing = await _announcementRepository.GetByIdAsync(announcementId);
            if (existing == null)
            {
                throw new ArgumentException("Announcement not found.");
            }

            await _announcementRepository.DeleteAsync(announcementId);

            if (existing.IsActive)
            {
                await _notificationService.RevokeAnnouncementNotificationsAsync(announcementId);
            }
        }

        public async Task SetActiveAsync(long announcementId, bool isActive)
        {
            if (announcementId <= 0)
            {
                throw new ArgumentException("A valid announcement id is required.", nameof(announcementId));
            }

            var existing = await _announcementRepository.GetByIdAsync(announcementId);
            if (existing == null)
            {
                throw new ArgumentException("Announcement not found.");
            }

            if (existing.IsActive == isActive)
            {
                return;
            }

            var updated = await _announcementRepository.SetActiveAsync(announcementId, isActive);
            if (!updated)
            {
                throw new ArgumentException("Announcement not found.");
            }

            await SyncNotificationDeliveryAsync(
                announcementId,
                existing.IsActive,
                isActive,
                existing.Title ?? "",
                existing.ShortMessage ?? existing.Title ?? "");
        }

        private Task DeliverAnnouncementNotificationsAsync(long announcementId, CreateAnnouncementRequest request) =>
            _notificationService.CreateAnnouncementNotificationsAsync(
                announcementId,
                request.Title!,
                request.ShortMessage ?? request.Title!);

        private async Task SyncNotificationDeliveryAsync(
            long announcementId,
            bool wasActive,
            bool isActive,
            string title,
            string shortMessage)
        {
            if (wasActive && !isActive)
            {
                await _notificationService.RevokeAnnouncementNotificationsAsync(announcementId);
                return;
            }

            if (!wasActive && isActive)
            {
                await _notificationService.RestoreAnnouncementNotificationsAsync(
                    announcementId,
                    title,
                    shortMessage);
            }
        }

        private static void ValidateRequest(CreateAnnouncementRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Title is required.", nameof(request.Title));
            }

            if (!request.StartsAt.HasValue || !request.ExpiresAt.HasValue)
            {
                throw new ArgumentException("starts_at and expires_at are required.");
            }

            if (request.ExpiresAt <= request.StartsAt)
            {
                throw new ArgumentException("expires_at must be after starts_at.");
            }
        }

        public Task<AnnouncementModel?> GetByIdAsync(long announcementId)
        {
            if (announcementId <= 0)
            {
                throw new ArgumentException("A valid announcement id is required.", nameof(announcementId));
            }

            return _announcementRepository.GetByIdAsync(announcementId);
        }

        public Task<AnnouncementModel?> GetLatestUnseenAsync(long userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("A valid user id is required.", nameof(userId));
            }

            return _announcementRepository.GetLatestUnseenAsync(userId);
        }

        public Task MarkAsSeenAsync(long announcementId, long userId)
        {
            if (announcementId <= 0)
            {
                throw new ArgumentException("A valid announcement id is required.", nameof(announcementId));
            }

            if (userId <= 0)
            {
                throw new ArgumentException("A valid user id is required.", nameof(userId));
            }

            return _announcementRepository.MarkAsSeenAsync(announcementId, userId);
        }
    }
}