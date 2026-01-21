namespace SoftBook.Domain.DTOs
{
    public class TaxGroupDetailDTO
    {
        public int TaxGroupDetailId { get; set; }
        public int TaxGroupId { get; set; }
        public int TaxId { get; set; }
        public int ApplyOrder { get; set; }
        public string? TaxName { get; set; }
        public decimal? RatePercent { get; set; }
        public string? TaxType { get; set; }
        public string? GroupName { get; set; }
    }
}
