using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class CurrencyDTO
    {
        public int CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? CurrencyName { get; set; }
        public byte? DecimalPlaces { get; set; }
        public string? Format { get; set; }
    }
}
