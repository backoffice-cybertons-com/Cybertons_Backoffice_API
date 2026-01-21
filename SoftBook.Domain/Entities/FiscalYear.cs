using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabFiscalYearMaster")]
    public class FiscalYear
    {
        [Key]
        public int FiscalYearId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public bool? IsCurrent { get; set; }
    }
}
