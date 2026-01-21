using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabPartyTypeMaster")]
    public class PartyTypeMaster
    {
        [Key]
        public int PartyTypeId { get; set; }

        public string PartyTypeCode { get; set; } = string.Empty;

        public string PartyTypeName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsCustomer { get; set; }

        public bool IsVendor { get; set; }

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}




