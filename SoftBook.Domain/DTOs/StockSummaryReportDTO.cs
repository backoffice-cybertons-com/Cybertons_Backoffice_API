namespace SoftBook.Domain.DTOs
{
    // Created by Hetak on 08052026
    public class StockSummaryReportDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal OpeningQty { get; set; }
        public decimal InwardQty { get; set; }
        public decimal CostPrice { get; set; }
        public decimal InwardAmount { get; set; }
        public decimal OutwardQty { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OutwardAmount { get; set; }
        public decimal ClosingQty { get; set; }
        public decimal ClosingAmount { get; set; }
    }
}
