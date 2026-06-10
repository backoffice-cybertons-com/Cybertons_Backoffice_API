namespace SoftBook.Domain.DTOs
{
    public class PartyRateAdjustmentSummaryItemDto
    {
        public int PartyId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string PartyType { get; set; } = string.Empty;
        public string? PaymentTermName { get; set; }
        public int? PaymentTermDays { get; set; }
        public decimal Last7DaysSalesTotal { get; set; }
        public decimal AvgSalePerDay { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal AvgOutstandingDays { get; set; }
        public decimal FromDue { get; set; }
        public decimal ToDue { get; set; }
        public decimal AddPrice { get; set; }
        public decimal StoredRateDiff { get; set; }
    }
}
