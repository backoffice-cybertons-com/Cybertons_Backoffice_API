using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
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

    public string? ReportPartyType { get; set; }

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
    public string? PartyProfile { get; set; }
    public decimal? RateDiff { get; set; }
    public string? KamMobile { get; set; }
    public string? RecoveryMobile { get; set; }
    public string? SumulIncharge { get; set; }
    [Column("sumul_incharge_mobile")]
    public string? SumulInchargeMobile { get; set; }
    public string? RetailerCode { get; set; }
    public string? SumulDepoCode { get; set; }
    
    [Column("salesperson_id")]
    public int? SalespersonId { get; set; }
    [Column("salesperson_mobile")]
    public string? SalespersonMobile { get; set; }
    [Column("kam")]
    public string? Kam { get; set; }
    [Column("recovery_agent")]
    public string? RecoveryAgent { get; set; }
    [Column("transporter_id")]
    public int? TransporterId { get; set; }
    [Column("transporter_mobile")]
    public string? TransporterMobile { get; set; }
    [Column("zone_id")]
    public int? ZoneId { get; set; }
    
    public string? PanDocPath { get; set; }
    public string? AadhaarDocPath { get; set; }
    public string? GstCertPath { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
  }
}
