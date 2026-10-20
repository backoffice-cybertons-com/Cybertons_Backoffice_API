namespace SoftBook.Domain.DTOs.GstIrn;

public class GstIrnSettingsDto
{
    public string Mode { get; set; } = "Mock";
    public string? AuthUrl { get; set; }
    public string? EInvoiceUrl { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ApiUsername { get; set; }
    public string? ApiPassword { get; set; }
    public string? PublicKey { get; set; }
    public string? Gstin { get; set; }
}

public class GstIrnGenerateResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Mode { get; set; } = "Mock";
    public int SpUniqueCode { get; set; }
    public string? InvNo { get; set; }
    public string? Irn { get; set; }
    public long? AckNo { get; set; }
    public DateTimeOffset? AckDt { get; set; }
    public string? SignedQrCode { get; set; }
    public string? QrImageBase64 { get; set; }
    public string? PayloadJson { get; set; }
    public string? ErrorCode { get; set; }
}

public class GstIrnStatusDto
{
    public int SpUniqueCode { get; set; }
    public string? InvNo { get; set; }
    public string? Irn { get; set; }
    public long? AckNo { get; set; }
    public DateTimeOffset? AckDt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public string? QrImageBase64 { get; set; }
    public bool IsMock { get; set; }
}
