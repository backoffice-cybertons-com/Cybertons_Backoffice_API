using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
  [Table("TabTCSMaster")]
  public class TCS
  {
    [Key]
    public int TCSId { get; set; }

    [Required]
    public decimal Rate { get; set; }

    [Required]
    public string? TaxName { get; set; }

    [Required]
    public string NatureOfCollection { get; set; } = string.Empty;

    public bool IsHigherRate { get; set; } = false;


    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    //public DateTime? CreatedAt { get; set; }
  }
}
