using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabPaymentTermMaster")]
    public class PaymentTerm
    {
        [Key]
        public int TermId { get; set; }
        
        public string? TermName { get; set; }
        public int? NumberOfDays { get; set; }
    }
}
