using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.Entities
{
    [Table("tab_delivery_challan_items", Schema = "public")]
    public class DeliveryChallanItem
    {
        [Key]
        [Column("item_id")]
        [JsonPropertyName("itemId")]
        public int ItemId { get; set; }

        [Column("company_id")]
        [JsonPropertyName("companyId")]
        public int CompanyId { get; set; }

        [Column("delivery_challan_id")]
        [JsonPropertyName("deliveryChallanId")]
        public int? DeliveryChallanId { get; set; }

        [Column("product_id")]
        [JsonPropertyName("productId")]
        public int? ProductId { get; set; }

        [Column("item_details")]
        [JsonPropertyName("itemDetails")]
        public string? ItemDetails { get; set; }

        [Required]
        [Column("quantity")]
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

        [Required]
        [Column("rate")]
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }

        [Column("tax_id")]
        [JsonPropertyName("taxId")]
        public int? TaxId { get; set; }

        [Required]
        [Column("amount")]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [Column("discount_pct")]
        [JsonPropertyName("discountPct")]
        public decimal? DiscountPct { get; set; }

        [Column("adjustment")]
        [JsonPropertyName("adjustment")]
        public decimal? Adjustment { get; set; }
    }
}
