using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabSalespersonMaster")]
    public class Salesperson
    {
        [Key]
        public int SalespersonId { get; set; }
        
        public string? SalespersonName { get; set; }
        public string? Email { get; set; }
    }
}
