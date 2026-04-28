using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabWarehouseMaster")]
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }
        
        [Required]
        public string WarehouseName { get; set; } = string.Empty;
        
        public int? BranchId { get; set; }
        public string? Address { get; set; }
    }
}
