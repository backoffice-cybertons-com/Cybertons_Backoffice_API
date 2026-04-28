using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;

namespace SoftBook.Domain.DTOs
{
    public class TDSDTO
    {
        public int TDSId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string Section { get; set; } = string.Empty;
        public bool IsHigherRate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
