using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabPartyAddress")]
    public class PartyAddress
    {
        [Key]
        public int AddressId { get; set; }

        public int PartyId { get; set; }

        [Required]
        public string AddressType { get; set; } = string.Empty; // Billing, Shipping, etc.

        public string? Attention { get; set; }
        public string? Country { get; set; }
        public string? Street1 { get; set; }
        public string? Street2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? Phone { get; set; }
        public string? FaxNumber { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}




