using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabTDSMaster")]
    public class TDS
    {
        [Key]
        public int TDSId { get; set; }
        
        [Required]
        public string TaxName { get; set; } = string.Empty;
        
        [Required]
        public decimal Rate { get; set; }
        
        [Required]
        public string Section { get; set; } = string.Empty;
        
        public bool IsHigherRate { get; set; } = false;
    public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
