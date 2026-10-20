namespace SoftBook.Domain.DTOs.GstIrn;

public class GstIrnOptions
{
    public const string SectionName = "GstIrn";

    /// <summary>Mock | Live</summary>
    public string Mode { get; set; } = "Mock";

    public string AuthUrl { get; set; } = string.Empty;
    public string EInvoiceUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string ApiUsername { get; set; } = string.Empty;
    public string ApiPassword { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string Gstin { get; set; } = string.Empty;
}
