using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.Entities
{
    [Table("tab_company_profile", Schema = "public")]
    public class CompanyProfile
    {
        [Key]
        [Column("company_id")]
        [JsonPropertyName("companyId")]
        public int CompanyId { get; set; }

        [Column("company_name")]
        [JsonPropertyName("companyName")]
        public string? CompanyName { get; set; }

        [Column("company_address")]
        [JsonPropertyName("companyAddress")]
        public string? CompanyAddress { get; set; }

        [Column("mobile")]
        [JsonPropertyName("mobile")]
        public string? Mobile { get; set; }

        [Column("phone")]
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [Column("email")]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [Column("gstin")]
        [JsonPropertyName("gstin")]
        public string? Gstin { get; set; }

        [Column("pan")]
        [JsonPropertyName("pan")]
        public string? Pan { get; set; }

        [Column("state")]
        [JsonPropertyName("state")]
        public string? State { get; set; }

        [Column("bank_name")]
        [JsonPropertyName("bankName")]
        public string? BankName { get; set; }

        [Column("bank_branch")]
        [JsonPropertyName("bankBranch")]
        public string? BankBranch { get; set; }

        [Column("account_number")]
        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        [Column("ifsc_code")]
        [JsonPropertyName("ifscCode")]
        public string? IfscCode { get; set; }

        [Column("logo_path")]
        [JsonPropertyName("logoPath")]
        public string? LogoPath { get; set; }

        [Column("created_at")]
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
