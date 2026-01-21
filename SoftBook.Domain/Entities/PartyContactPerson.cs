using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabPartyContactPersons")]
    public class PartyContactPerson
    {
        [Key]
        public int ContactPersonId { get; set; }

        public int PartyId { get; set; }

        public string? Salutation { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? WorkPhone { get; set; }
        public string? Mobile { get; set; }
        public bool? IsPrimary { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}




