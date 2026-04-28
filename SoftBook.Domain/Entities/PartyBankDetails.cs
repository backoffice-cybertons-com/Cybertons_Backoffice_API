using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabPartyBankDetails")]
    public class PartyBankDetails
    {
        [Key]
        public int BankDetailId { get; set; }

        public int PartyId { get; set; }

        [Required]
        public string AccountHolderName { get; set; } = string.Empty;

        [Required]
        public string BankName { get; set; } = string.Empty;

        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        public string IFSC { get; set; } = string.Empty;

        public string? Branch { get; set; }
        public string? MICR { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}




