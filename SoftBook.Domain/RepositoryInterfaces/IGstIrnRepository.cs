using SoftBook.Domain.DTOs.GstIrn;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.RepositoryInterfaces;

public interface IGstIrnRepository
{
    Task<GstIrnSettingsDto?> GetSettingsAsync(int companyId);
    Task SaveSettingsAsync(int companyId, GstIrnSettingsDto settings);
    Task<GstIrnLog?> GetBySpUniqueCodeAsync(int companyId, int spUniqueCode);
    Task SaveIrnLogAsync(GstIrnLog log);
}
