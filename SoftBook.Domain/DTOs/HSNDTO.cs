using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class HSNDTO
    {
        public int HSNId { get; set; }
        public string? HSNCode { get; set; }
        public string? Description { get; set; }
        public decimal? TaxRate { get; set; }
    }
}
