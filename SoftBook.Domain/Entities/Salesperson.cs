using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabSalespersonMaster")]
    public class Salesperson
    {
        [Key]
        public int SalespersonId { get; set; }
        
        public string? SalespersonName { get; set; }
        public string? Email { get; set; }
        
        [Column("gstin")]
        public string? Gstin { get; set; }
        [Column("contact_person")]
        public string? ContactPerson { get; set; }
        [Column("phone")]
        public string? Phone { get; set; }
        [Column("address")]
        public string? Address { get; set; }
    }
}
