namespace SoftBook.Domain.DTOs
{
    public class BalanceSheetReportDTO
    {
        public string? ReportSide { get; set; }
        public string? RowType { get; set; }
        public string? GroupName { get; set; }
        public long? LedgerId { get; set; }
        public string? LedgerName { get; set; }
        public decimal Amount { get; set; }

        public long AccountId { get; set; }
    }
}
