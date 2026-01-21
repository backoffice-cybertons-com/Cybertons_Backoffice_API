using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabRoleMaster")]
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        
        public string? RoleName { get; set; }
    }
}
