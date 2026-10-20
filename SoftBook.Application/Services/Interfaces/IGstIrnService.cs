using SoftBook.Domain.DTOs.GstIrn;

namespace SoftBook.Application.Services.Interfaces;

public interface IGstIrnService
{
    Task<GstIrnSettingsDto> GetSettingsAsync(int companyId);
    Task SaveSettingsAsync(int companyId, GstIrnSettingsDto settings);
    Task<string> BuildPayloadJsonAsync(int companyId, int spUniqueCode);
    Task<GstIrnGenerateResultDto> GenerateAsync(int companyId, int spUniqueCode, int? userId);
    Task<GstIrnStatusDto?> GetStatusAsync(int companyId, int spUniqueCode);
}
