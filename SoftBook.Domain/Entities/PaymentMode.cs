using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabPaymentModeMaster")]
    public class PaymentMode
    {
        [Key]
        public int PaymentModeId { get; set; }
        public string? ModeName { get; set; }
        public bool IsDefault { get; set; }
    }
}
