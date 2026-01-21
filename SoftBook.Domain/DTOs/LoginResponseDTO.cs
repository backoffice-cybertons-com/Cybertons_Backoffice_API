using SoftBook.Domain.Entities;

namespace SoftBook.Domain.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public User User { get; set; } = new();
    }
}
