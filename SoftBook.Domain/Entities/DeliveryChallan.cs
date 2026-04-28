using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.Entities
{
    [Table("tab_delivery_challan", Schema = "public")]
    public class DeliveryChallan
    {
        [Key]
        [Column("delivery_challan_id")]
        [JsonPropertyName("deliveryChallanId")]
        public int DeliveryChallanId { get; set; }

        [Column("company_id")]
        [JsonPropertyName("companyId")]
        public int CompanyId { get; set; }

        [Required]
        [Column("customer_id")]
        [JsonPropertyName("customerId")]
        public int CustomerId { get; set; }

        [Column("location_id")]
        [JsonPropertyName("locationId")]
        public int? LocationId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("challan_number")]
        [JsonPropertyName("challanNumber")]
        public string ChallanNumber { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("reference_no")]
        [JsonPropertyName("referenceNo")]
        public string? ReferenceNo { get; set; }

        [Required]
        [Column("challan_date")]
        [JsonPropertyName("challanDate")]
        public DateTime ChallanDate { get; set; }

        [Required]
        [StringLength(50)]
        [Column("challan_type")]
        [JsonPropertyName("challanType")]
        public string ChallanType { get; set; } = string.Empty;

        [Column("customer_notes")]
        [JsonPropertyName("customerNotes")]
        public string? CustomerNotes { get; set; }

        [Column("terms")]
        [JsonPropertyName("terms")]
        public string? Terms { get; set; }

        [StringLength(250)]
        [Column("warehouse_location")]
        [JsonPropertyName("warehouseLocation")]
        public string? WarehouseLocation { get; set; }
    }
}
