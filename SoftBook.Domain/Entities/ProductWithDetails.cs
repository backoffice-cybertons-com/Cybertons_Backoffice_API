namespace SoftBook.Domain.Entities
{
  public class ProductWithDetails
  {
    public int ProductId { get; set; }
    public string? Type { get; set; }
    public string? Name { get; set; }

    // Prices (mapped from SP column names)
    public decimal? SellingPrice { get; set; } // Maps to SalesRate from SP
    public decimal? CostPrice { get; set; } // Maps to PurchaseRate from SP

    // Descriptions
    public string? SalesDescription { get; set; }
    public string? PurchaseDescription { get; set; }

    // Unit
    public string? UnitName { get; set; } // Maps to UsageUnit from SP

    // HSN/SAC
    public string? HSNCode { get; set; }
    public string? SACCode { get; set; }

    // Tax
    public int? TaxPreferenceId { get; set; }
    public int? TaxGroupId { get; set; }

    // Computed property for HSN/SAC display
    public string? HSNOrSACCode => Type == "Goods" ? HSNCode : SACCode;
  }
}

