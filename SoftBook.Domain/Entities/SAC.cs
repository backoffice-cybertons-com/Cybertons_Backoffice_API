using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabSACMaster")]
    public class SAC
    {
        [Key]
        public int SACId { get; set; }
        
        public string? SACCode { get; set; }
        public string? Description { get; set; }
        public decimal? TaxRate { get; set; }
    }
}
