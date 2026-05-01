using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
  public class ProductWithDetails
  {
    public int ProductId { get; set; }
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? ShortName { get; set; }

    // Prices
    public decimal? MRP { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? SellingPrice2 { get; set; }
    public decimal? SellingPrice3 { get; set; }
    public decimal? SellingPrice4 { get; set; }
    public decimal? SellingPrice5 { get; set; }
    public decimal? CostPrice { get; set; }

    // Descriptions
    public string? SalesDescription { get; set; }
    public string? PurchaseDescription { get; set; }

    // Display Details
    public string? BaseUnit { get; set; }
    public string? PackingUnit { get; set; }
    public string? ProductGroupName { get; set; }
    public string? TaxGroupName { get; set; }

    // HSN/SAC
    public string? HSNCode { get; set; }
    public string? SACCode { get; set; }

    // Tax
    public int? TaxPreferenceId { get; set; }
    public int? TaxGroupId { get; set; }

    // Inventory
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public decimal? ReOrderQuantity { get; set; }

    public decimal? OpeningQuantity { get; set; }
    public decimal? OpeningPrice { get; set; }
    public decimal? OpeningAmount { get; set; }
    public string? PreferredVendorName { get; set; }

    // New fields for Purchase Transaction

    public decimal? CGSTPer { get; set; }
    public decimal? SGSTPer { get; set; }
    public int? UnitId { get; set; }
    public int? ProductPackingUnit { get; set; }

    // Computed property for HSN/SAC display
    public string? HSNOrSACCode => Type == "Goods" ? HSNCode : SACCode;
  }
}

