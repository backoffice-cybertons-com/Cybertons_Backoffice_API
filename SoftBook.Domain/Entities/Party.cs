using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
  [Table("TabPartyMaster")]
  public class Party
  {
    [Key]
    public int PartyId { get; set; }

    [Required]
    public string PartyType { get; set; } = string.Empty;


    public int? PartyTypeId { get; set; }

    public string? Salutation { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? PhoneWork { get; set; }
    public string? PhoneMobile { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? GSTTreatment { get; set; }
    public string? PlaceOfSupply { get; set; }
    public string? PAN { get; set; }
    public string? GSTNO { get; set; }
    public bool? MSMERegistered { get; set; }
    public string? TaxPreference { get; set; }
    public string? TDS { get; set; }
    public int? Currency { get; set; }
    public int? AccountsReceivableAccount { get; set; }
    public int? AccountsPayableAccount { get; set; }
    public decimal? OpeningBalance { get; set; }
    public string? OpeningBalanceType { get; set; }
    public string? PaymentTerms { get; set; }
    public bool? EnablePortal { get; set; }
    public string? WebsiteURL { get; set; }
    public string? Department { get; set; }
    public string? Remarks { get; set; }
    public DateTime? CreatedOn { get; set; }
  }
}
