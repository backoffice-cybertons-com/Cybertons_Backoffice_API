using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabUnitMaster")]
    public class Unit
    {
        [Key]
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public string? UQC { get; set; }
    }
}
