// Developed by Hetak on 11-05-2026
namespace SoftBook.Domain.DTOs
{
    public class StockLedgerDatewiseDTO
    {
        public long SrNo { get; set; }
        public DateTime? TranDate { get; set; }
        public string? Description { get; set; }
        public decimal InwardQty { get; set; }
        public decimal InwardAmount { get; set; }
        public decimal OutwardQty { get; set; }
        public decimal OutwardAmount { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal BalanceAmount { get; set; }
    }
}

