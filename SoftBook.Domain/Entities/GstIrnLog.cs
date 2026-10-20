using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities;

[Table("tab_gst_irn", Schema = "public")]
public class GstIrnLog
{
    [Key]
    [Column("irn_id")]
    public int IrnId { get; set; }

    [Column("sp_unique_code")]
    public int SpUniqueCode { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("inv_no")]
    public string? InvNo { get; set; }

    [Column("ack_no")]
    public long? AckNo { get; set; }

    [Column("ack_dt")]
    public DateTimeOffset? AckDt { get; set; }

    [Column("irn")]
    public string? Irn { get; set; }

    [Column("signed_invoice")]
    public string? SignedInvoice { get; set; }

    [Column("signed_qr_code")]
    public string? SignedQrCode { get; set; }

    [Column("qr_image")]
    public byte[]? QrImage { get; set; }

    [Column("status")]
    public string Status { get; set; } = "SUCCESS";

    [Column("is_mock")]
    public bool IsMock { get; set; }

    [Column("error_code")]
    public string? ErrorCode { get; set; }

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("created_on")]
    public DateTime? CreatedOn { get; set; }
}
