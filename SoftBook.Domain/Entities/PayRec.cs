using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.Entities
{
    [Table("tab_pay_rec", Schema = "public")]
    public class PayRec
    {
        [Key]
        [Column("code")]
        public long Code { get; set; }

        [Column("date")]
        public DateTime? Date { get; set; }

        [Column("inv_code")]
        public long? InvCode { get; set; }

        [Column("jv_code")]
        public long? JvCode { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("disc")]
        public string? Disc { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("created_on")]
        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        [Column("user_code")]
        public int? UserCode { get; set; }

        [Column("post_cheque")]
        public long? PostCheque { get; set; }

        [Column("company_id")]
        public long? CompanyId { get; set; }

        [Column("full_half")]
        public string? FullHalf { get; set; }

        [Column("year_code")]
        public int? YearCode { get; set; }

        [Column("branch_code")]
        public int? BranchCode { get; set; }

        [Column("vch_type")]
        public string? VchType { get; set; }

        [Column("deleted_by")]
        public string? DeletedBy { get; set; }

        [Column("deleted_on")]
        public DateTime? DeletedOn { get; set; }

        [Column("delete_flag")]
        public bool DeleteFlag { get; set; } = false;

        [Column("party_id")]
        [JsonPropertyName("partyId")]
        public long? PartyId { get; set; }

        [Column("account_id")]
        [JsonPropertyName("accountId")]
        public long? AccountId { get; set; }

        [Column("discount_amount")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("g_code")]
        public string? GCode { get; set; }
    }
}
