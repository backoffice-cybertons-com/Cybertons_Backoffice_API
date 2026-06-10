using System;

namespace SoftBook.Domain.DTOs
{
    public class PartyInvoiceAddPriceMismatchDto
    {
        public int SpUniqueCode { get; set; }
        public string InvNo { get; set; } = string.Empty;
        public DateTime InvDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int PartyId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string PartyType { get; set; } = string.Empty;
        public decimal ExpectedAddPrice { get; set; }
        public decimal ActualAddPrice { get; set; }
        public decimal Difference { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal Last7DaysSalesTotal { get; set; }
        public decimal AvgSalePerDay { get; set; }
        public decimal AvgOutstandingDays { get; set; }
        public decimal FromDue { get; set; }
        public decimal ToDue { get; set; }
    }
}
