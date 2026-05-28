using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Interfaces
{
    public interface IAnnouncementService
    {
        Task<long> CreateAsync(CreateAnnouncementRequest request);
        Task<AnnouncementModel?> GetByIdAsync(long announcementId);
        Task<List<AnnouncementModel>> GetAllAsync();
        Task UpdateAsync(long announcementId, UpdateAnnouncementRequest request);
        Task DeleteAsync(long announcementId);
        Task SetActiveAsync(long announcementId, bool isActive);
        Task<AnnouncementModel?> GetLatestUnseenAsync(long userId);
        Task MarkAsSeenAsync(long announcementId, long userId);
    }
}
