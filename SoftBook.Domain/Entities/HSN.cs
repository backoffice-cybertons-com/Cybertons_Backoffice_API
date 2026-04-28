using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabHSNMaster")]
    public class HSN
    {
        [Key]
        public int HSNId { get; set; }
        public string? HSNCode { get; set; }
        public string? Description { get; set; }
        public decimal? TaxRate { get; set; }
    }
}
