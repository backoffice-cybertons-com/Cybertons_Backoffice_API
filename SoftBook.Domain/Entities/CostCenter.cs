using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabCostCenterMaster")]
    public class CostCenter
    {
        [Key]
        public int CostCenterId { get; set; }
        
        [Required]
        public string CostCenterName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
    }
}
