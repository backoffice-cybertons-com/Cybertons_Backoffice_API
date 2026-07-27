namespace SoftBook.Domain.DTOs
{
    public class PartyBulkSummaryDto
    {
        public int PartyId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string PartyType { get; set; } = string.Empty;
        public string? PaymentTerms { get; set; }
        public decimal StoredRateDiff { get; set; }
        public decimal Last7DaysSalesTotal { get; set; }
        public decimal TotalOutstanding { get; set; }
    }
}
