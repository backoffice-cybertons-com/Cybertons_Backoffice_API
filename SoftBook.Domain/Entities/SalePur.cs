using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.Entities
{
    [Table("tab_sale_pur", Schema = "public")]
    public class SalePur
    {
        [Key]
        [Column("sp_unique_code")]
        [JsonPropertyName("spUniqueCode")]
        public int SpUniqueCode { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("invoice_series")]
        public string? InvoiceSeries { get; set; }

        [Column("invoice_series_increment_number")]
        public int? InvoiceSeriesIncrementNumber { get; set; }

        [Column("inv_type")]
        public string InvType { get; set; } = string.Empty;

        [Column("inv_sub_type")]
        public string? InvSubType { get; set; }

        [Column("inv_category")]
        public string? InvCategory { get; set; }

        [Column("sales_order_no")]
        public string? SalesOrderNo { get; set; }

        [Column("sales_order_date")]
        public DateTime? SalesOrderDate { get; set; }

        [Column("po_order_no")]
        public string? PoOrderNo { get; set; }

        [Column("po_order_date")]
        public DateTime? PoOrderDate { get; set; }

        [Column("bill_id")]
        public int? BillId { get; set; }

        [Column("delivery_no")]
        public int? DeliveryNo { get; set; }

        [Column("delivery_date")]
        public DateTime? DeliveryDate { get; set; }

        [Column("vehicle_no")]
        public string? VehicleNo { get; set; }

        [Column("transporter_id")]
        public int? TransporterId { get; set; }

        [Column("unit_id")]
        public int? UnitId { get; set; }

        [Column("bank_id")]
        public int? BankId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Column("tax_id")]
        public int? TaxId { get; set; }

        [Column("net_invoice_amount")]
        public decimal? NetInvoiceAmount { get; set; }

        [Column("grand_total")]
        public decimal? GrandTotal { get; set; }

        [Column("billing_address")]
        public string? BillingAddress { get; set; }

        [Column("billing_address1")]
        public string? BillingAddress1 { get; set; }

        [Column("billing_address2")]
        public string? BillingAddress2 { get; set; }

        [Column("shipping_address")]
        public string? ShippingAddress { get; set; }

        [Column("shipping_address1")]
        public string? ShippingAddress1 { get; set; }

        [Column("shipping_address2")]
        public string? ShippingAddress2 { get; set; }

        [Column("tot_discount")]
        public decimal TotDiscount { get; set; } = 0;

        [Column("tot_sku_in_invoice")]
        public int TotSkuInInvoice { get; set; } = 0;

        [Column("inv_no")]
        public string? InvNo { get; set; }

        [Column("inv_date")]
        public DateTime InvDate { get; set; }

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        [Column("party_id")]
        [JsonPropertyName("partyId")]
        public int PartyId { get; set; }

        [Column("terms_id")]
        public int? TermsId { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("taxable_amount")]
        public decimal? TaxableAmount { get; set; }

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

        [Column("round_off")]
        public decimal? RoundOff { get; set; }

        [Column("previous_outstanding")]
        public decimal? PreviousOutstanding { get; set; }

        [Column("current_bill_amount")]
        public decimal? CurrentBillAmount { get; set; }

        [Column("final_outstanding")]
        public decimal? FinalOutstanding { get; set; }

        [Column("jurisdiction")]
        public string? Jurisdiction { get; set; }

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


		[NotMapped]
		[JsonPropertyName("todaysReceipt")]
		public decimal? TodaysReceipt { get; set; }

        [Column("invoice_total")]
        public decimal? InvoiceTotal { get; set; }

        [Column("final_invoice_amount")]
        public decimal? FinalInvoiceAmount { get; set; }


    }
}
