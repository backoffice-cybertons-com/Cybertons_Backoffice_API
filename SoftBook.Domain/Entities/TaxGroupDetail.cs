using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabTaxGroupDetail")]
    public class TaxGroupDetail
    {
        [Key]
        public int TaxGroupDetailId { get; set; }
        
        public int TaxGroupId { get; set; }
        public int TaxId { get; set; }
        public int ApplyOrder { get; set; }
        
        // Navigation properties for display
        public string? TaxName { get; set; }
        public decimal? RatePercent { get; set; }
        public string? TaxType { get; set; }
        public string? GroupName { get; set; }
    }
}
