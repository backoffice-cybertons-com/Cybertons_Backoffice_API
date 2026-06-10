namespace SoftBook.Domain.DTOs
{
    public class PartyRateAdjustmentResultDto
    {
        public int PartyId { get; set; }
        public decimal Last7DaysSalesTotal { get; set; }
        public decimal AvgSalePerDay { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal AvgOutstandingDays { get; set; }
        public decimal AddPrice { get; set; }
        public decimal FromDue { get; set; }
        public decimal ToDue { get; set; }
        public decimal RateDiff { get; set; }
        /// <summary>Current value stored in tab_party_master.rate_diff.</summary>
        public decimal StoredRateDiff { get; set; }
        public bool Updated { get; set; }
    }
}
