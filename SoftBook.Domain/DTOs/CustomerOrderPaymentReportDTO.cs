using System;

namespace SoftBook.Domain.DTOs
{
    /// <summary>
    /// Customer Yesterday OS / Yesterday Order / Today Payment report row.
    /// TodayOrder is computed for on-screen display; Excel export sets it as a formula.
    /// </summary>
    public class CustomerOrderPaymentReportDTO
    {
        public int PartyId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime YesterdayDate { get; set; }

        /// <summary>Outstanding as of start of report date (end of yesterday).</summary>
        public decimal YesterdayOs { get; set; }

        /// <summary>Sales grand total on yesterday from tab_sale_pur.</summary>
        public decimal YesterdayOrder { get; set; }

        /// <summary>
        /// Yesterday OS / Yesterday Order. Excel export uses formula =B/C.
        /// </summary>
        public decimal TermDays { get; set; }

        /// <summary>Income (direct) JV credits to party on report date from tab_jv.</summary>
        public decimal TodayPayment { get; set; }

        /// <summary>
        /// Display value: IF(TodayPayment &gt; YesterdayOrder, TodayPayment, TodayPayment + (TodayPayment - YesterdayOrder)).
        /// Excel export uses the same as a cell formula.
        /// </summary>
        public decimal TodayOrder { get; set; }
    }
}
