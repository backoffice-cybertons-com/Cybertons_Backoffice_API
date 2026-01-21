namespace SoftBook.Domain.DTOs
{
    public class BranchDTO
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }
    }
}
