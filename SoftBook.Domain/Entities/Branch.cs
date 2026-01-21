using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabBranchMaster")]
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        
        [Required]
        public string BranchName { get; set; } = string.Empty;
        
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }
    }
}
