using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabGodownMaster")]
    public class Godown
    {
        [Key]
        public int GodownId { get; set; }
        
        public int? WarehouseId { get; set; }
        public string? GodownName { get; set; }
    }
}
