using System;

namespace SoftBook.Domain.DTOs
{
    public class CalendarSummaryDto
    {
        public DateTime ReportDate { get; set; }

        public decimal TotalSale { get; set; }

        public decimal TotalReceipt { get; set; }

        public decimal TotalOs { get; set; }
        public decimal TotalAdj { get; set; }



        public string BgColor { get; set; }
        public decimal AvgTerms { get; set; }
    }
}
