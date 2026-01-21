namespace SoftBook.Domain.DTOs
{
    public class WarehouseDTO
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string? Address { get; set; }
    }
}
