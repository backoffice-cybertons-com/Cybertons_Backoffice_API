using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabUserMaster")]
    public class User
    {
        [Key]
        [Column("UserId")]
        public int Id { get; set; }

        [NotMapped]
        public int CompanyId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = string.Empty;

        public string? FullName { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile No. should be 10 digits")]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
