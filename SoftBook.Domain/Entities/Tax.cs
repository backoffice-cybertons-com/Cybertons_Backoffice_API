using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabTaxMaster")]
    public class Tax
    {
        [Key]
        public int TaxId { get; set; }
        public string? TaxName { get; set; }
        public decimal? RatePercent { get; set; }
        public string? TaxType { get; set; }
    }
}
