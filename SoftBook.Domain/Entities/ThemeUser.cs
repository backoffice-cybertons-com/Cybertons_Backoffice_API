using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    public class ThemeUser
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool IsVerified { get; set; }

        [Required]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        public string SelectedRole { get; set; } = string.Empty;

        [Required]
        public string SelectedPlan { get; set; } = string.Empty;

        [NotMapped]
        public List<string> AvailableRoles { get; set; } = new();

        [NotMapped]
        public List<string> AvailablePlans { get; set; } = new();
    }
}

