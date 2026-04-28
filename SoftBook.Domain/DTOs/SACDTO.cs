using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class SACDTO
    {
        public int SACId { get; set; }
        public string? SACCode { get; set; }
        public string? Description { get; set; }
        public decimal? TaxRate { get; set; }
    }
}
