using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Domain.RepositoryInterfaces
{
    public interface IAnnouncementRepository
    {
        Task<long> CreateAsync(CreateAnnouncementRequest request);
        Task<AnnouncementModel?> GetByIdAsync(long announcementId);
        Task<List<AnnouncementModel>> GetAllAsync();
        Task<bool> UpdateAsync(long announcementId, UpdateAnnouncementRequest request);
        Task DeleteAsync(long announcementId);
        Task<bool> SetActiveAsync(long announcementId, bool isActive);
        Task<AnnouncementModel?> GetLatestUnseenAsync(long userId);
        Task MarkAsSeenAsync(long announcementId, long userId);
    }
}
