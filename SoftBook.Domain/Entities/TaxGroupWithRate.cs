using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.Entities
{
  public class TaxGroupWithRate
  {
    public int TaxGroupId { get; set; }
    public string? GroupName { get; set; }
    public string? SupplyType { get; set; }
    public decimal TotalRatePercent { get; set; }
    public string DisplayName => $"{GroupName} [{TotalRatePercent}%]";
  }
}

