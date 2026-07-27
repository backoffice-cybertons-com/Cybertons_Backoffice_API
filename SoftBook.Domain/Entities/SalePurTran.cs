using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.Entities
{
    [Table("tab_sal_pur_tran", Schema = "public")]
    public class SalePurTran
    {
        [Key]
        [Column("sp_tran_unique_code")]
        [JsonPropertyName("spTranUniqueCode")]
        public int SpTranUniqueCode { get; set; }

        [Required]
        [Column("sp_unique_code")]
        [JsonPropertyName("spUniqueCode")]
        public int SpUniqueCode { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("product_id")]
        [JsonPropertyName("productId")]
        public int? ProductId { get; set; }

        [Column("product_details")]
        public string? ProductDetails { get; set; }

        [Column("batch_date")]
        public DateTime? BatchDate { get; set; }

        [Column("batch_no")]
        public string? BatchNo { get; set; }

        [Column("mrp")]
        public decimal? Mrp { get; set; }

        [Column("sub_unit")]
        public int? SubUnit { get; set; }

        [Column("sub_unit_quantity")]
        public decimal? SubUnitQuantity { get; set; }

        [Column("unit")]
        public int? Unit { get; set; }

        [Required]
        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Required]
        [Column("rate")]
        public decimal Rate { get; set; }

        [Column("discount_per")]
        public decimal DiscountPer { get; set; } = 0;

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("igst_percent")]
        public decimal? IgstPercent { get; set; }

        [Column("igst_amount")]
        public decimal? IgstAmount { get; set; }

        [Column("cgst_percent")]
        public decimal? CgstPercent { get; set; }

        [Column("cgst_amount")]
        public decimal? CgstAmount { get; set; }

        [Column("sgst_percent")]
        public decimal? SgstPercent { get; set; }

        [Column("sgst_amount")]
        public decimal? SgstAmount { get; set; }

        [Column("crt")]
        public decimal? Crt { get; set; }

        [Column("packing")]
        public string? Packing { get; set; }

        [Column("hsn_code")]
        public string? HsnCode { get; set; }

        [Required]
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("fiscal_year_id")]
        public int? FiscalYearId { get; set; }

        [Column("branch_id")]
        public int? BranchId { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("modified_by")]
        public int? ModifiedBy { get; set; }

        [Column("deleted_by")]
        public int? DeletedBy { get; set; }

        [Column("delete_flag")]
        public bool? DeleteFlag { get; set; } = false;

        [Column("created_on")]
        public DateTime? CreatedOn { get; set; }

        [Column("modified_on")]
        public DateTime? ModifiedOn { get; set; }

        [Column("deleted_on")]
        public DateTime? DeletedOn { get; set; }

        [Column("rate_diff")]
        [JsonPropertyName("rateDiff")]
        public decimal? RateDiff { get; set; }

        [Column("cogs")]
        [JsonPropertyName("cogs")]
        public decimal? Cogs { get; set; }

        [Column("dealer_price")]
        [JsonPropertyName("dealerPrice")]
        public decimal? DealerPrice { get; set; }

        [Column("sp_comm_unit")]
        [JsonPropertyName("spCommUnit")]
        public decimal? SpCommUnit { get; set; }

        [NotMapped]
        [JsonPropertyName("companyCommUnit")]
        public decimal CompanyCommUnit => (Mrp ?? 0) - Rate - (SpCommUnit ?? 0);

        [NotMapped]
        [JsonPropertyName("invAmountMrp")]
        public decimal InvAmountMrp => Quantity * (Mrp ?? 0);

        [NotMapped]
        [JsonPropertyName("spCommTotal")]
        public decimal SpCommTotal => Quantity * (SpCommUnit ?? 0);

        [NotMapped]
        [JsonPropertyName("companyCommTotal")]
        public decimal CompanyCommTotal => Quantity * CompanyCommUnit;
    }
}
