using System;

namespace SoftBook.Domain.DTOs
{
    public class MonthlyCommissionReportDTO
    {
        public string DateOrParty { get; set; }
        public string InvNo { get; set; }
        public string ProductName { get; set; } // Either formatted Date or Party Name
        public decimal CrateOrdered { get; set; }
        public decimal TotalMrpOrderAmount { get; set; }
        public decimal AvgMrp { get; set; }
        public decimal SevenDaysAvgOrder { get; set; }
        public decimal AvgDepoPrice { get; set; }
        public decimal AvgDistAmount { get; set; }
        public decimal TotComm { get; set; }
        public decimal AvgCommPerCrate { get; set; }
        public decimal OsAsOn { get; set; }
        public decimal Payment { get; set; }
        public decimal AvgTerm { get; set; }
        public decimal AdjustCommission { get; set; }
        public decimal NetCommission { get; set; }
        public decimal CommAmount { get; set; }
    }
}
