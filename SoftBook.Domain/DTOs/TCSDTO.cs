using System;

namespace SoftBook.Domain.DTOs
{
    public class TCSDTO
    {
        public int TCSId { get; set; }
        public decimal Rate { get; set; }
        public string? TaxName { get; set; }
        public string NatureOfCollection { get; set; } = string.Empty;
        public bool IsHigherRate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
