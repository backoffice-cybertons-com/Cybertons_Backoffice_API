using System;

namespace SoftBook.Domain.DTOs
{
    // Code developed by Hetak Modi 290526
    public class DayBookReportDTO
    {
        public DateTime JvDate { get; set; }
        public long JvCode { get; set; }
        public string VType { get; set; }
        public string DrAccountName { get; set; }
        public string CrAccountName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
