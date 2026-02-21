namespace SoftBook.Infrastructure.Interfaces
{
    public interface ITenantProvider
    {
        int? CompanyId { get; set; }
        string? ConnectionString { get; set; }
    }

}
