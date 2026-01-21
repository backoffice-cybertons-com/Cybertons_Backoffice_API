namespace SoftBook.Domain.DTOs
{
    public class PaymentModeDTO
    {
        public int PaymentModeId { get; set; }
        public string? ModeName { get; set; }
        public bool IsDefault { get; set; }
    }
}
