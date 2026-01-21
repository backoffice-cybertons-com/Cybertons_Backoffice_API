namespace SoftBook.Infrastructure.Interfaces
{
    public interface ITenantProvider
    {
        int? TenantId { get; set; }
        string? ConnectionString { get; set; }
    }

    public class TenantProvider : ITenantProvider
    {
        public int? TenantId { get; set; }
        public string? ConnectionString { get; set; }
    }
}
