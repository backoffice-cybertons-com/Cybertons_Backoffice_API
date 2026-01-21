using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
  [Table("TabProductMaster")]
  public class Product
  {
    [Key]
    public int ProductId { get; set; }

    public string? Type { get; set; }

    public string? Name { get; set; }

    public decimal? SellingPrice { get; set; }

    public string? SalesAccount { get; set; }

    public string? SalesDescription { get; set; }

    public decimal? CostPrice { get; set; }

    public string? PurchaseAccount { get; set; }

    public string? PurchaseDescription { get; set; }

    public int? PreferredVendorId { get; set; }

    public bool Sellable { get; set; }

    public bool Purchasable { get; set; }

    public int? UnitId { get; set; }

    public int? HSNId { get; set; }

    public int? SACId { get; set; }

    public int? TaxPreferenceId { get; set; }

    public int? TaxGroupId { get; set; } // Intra State Tax Group

    public int? InterStateTaxGroupId { get; set; } // Inter State Tax Group

    public string? SupplyType { get; set; } // INTRA or INTER
  }
}
