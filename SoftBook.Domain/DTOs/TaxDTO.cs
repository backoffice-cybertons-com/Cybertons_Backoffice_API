using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class TaxDTO
    {
        public int TaxId { get; set; }
        public string? TaxName { get; set; }
        public decimal? RatePercent { get; set; }
        public string? TaxType { get; set; }
    }
}
