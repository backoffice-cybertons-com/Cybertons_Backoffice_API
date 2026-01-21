namespace SoftBook.Domain.DTOs
{
    public class CostCenterDTO
    {
        public int CostCenterId { get; set; }
        public string CostCenterName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
