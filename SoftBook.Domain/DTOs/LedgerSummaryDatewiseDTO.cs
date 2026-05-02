using System;

namespace SoftBook.Domain.DTOs
{
    public class LedgerSummaryDatewiseDTO
    {
        public DateTime? ResDate { get; set; }
        public string? ResParticular { get; set; }
        public decimal ResDebit { get; set; }
        public decimal ResCredit { get; set; }
        public decimal? ResBalance { get; set; }
    }
}
