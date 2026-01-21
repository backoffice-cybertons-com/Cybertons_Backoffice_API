namespace SoftBook.Domain.DTOs
{
    public class TaxDTO
    {
        public int TaxId { get; set; }
        public string? TaxName { get; set; }
        public decimal? RatePercent { get; set; }
        public string? TaxType { get; set; }
    }
}
