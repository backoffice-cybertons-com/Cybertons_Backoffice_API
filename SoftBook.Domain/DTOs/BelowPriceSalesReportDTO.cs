using System;

namespace SoftBook.Domain.DTOs
{
    public class BelowPriceSalesReportDTO
    {
        public int SpUniqueCode { get; set; }
        public DateTime InvDate { get; set; }
        public string? InvNo { get; set; }
        public string? PartyName { get; set; }
        public string? ProductName { get; set; }
        public decimal Qty { get; set; }
        public decimal InvoiceRate { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal LossVsCost { get; set; }
        public decimal DiffVsSellingPrice { get; set; }
        public bool IsBelowCost { get; set; }
        public bool IsBelowSellingPrice { get; set; }
        public string? AlertType { get; set; }
    }
}
