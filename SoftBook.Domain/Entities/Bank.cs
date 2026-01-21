using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabBankMaster")]
    public class Bank
    {
        [Key]
        public int BankMasterId { get; set; }
        
        [Required]
        public string BankName { get; set; } = string.Empty;
        
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSC { get; set; }
        public string? MICR { get; set; }
    }
}
