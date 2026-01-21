using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabProjectMaster")]
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        
        [Required]
        public string ProjectName { get; set; } = string.Empty;
        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
    }
}
