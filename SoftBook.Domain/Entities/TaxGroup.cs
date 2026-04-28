using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabTaxGroupMaster")]
    public class TaxGroup
    {
        [Key]
        public int TaxGroupId { get; set; }
        
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public string? SupplyType { get; set; } // INTRA or INTER
    }
}
