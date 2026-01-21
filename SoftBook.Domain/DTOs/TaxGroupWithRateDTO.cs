namespace SoftBook.Domain.DTOs
{
    public class TaxGroupWithRateDTO
    {
        public int TaxGroupId { get; set; }
        public string? GroupName { get; set; }
        public string? SupplyType { get; set; }
        public decimal TotalRatePercent { get; set; }
        public string DisplayName => $"{GroupName} [{TotalRatePercent}%]";
    }
}
