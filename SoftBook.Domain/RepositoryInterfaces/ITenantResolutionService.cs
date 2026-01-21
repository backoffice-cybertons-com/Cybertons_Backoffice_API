namespace SoftBook.Infrastructure.Interfaces
{
    public interface ITenantResolutionService
    {
        Task<string?> GetConnectionStringAsync(int tenantId);
    }
}
