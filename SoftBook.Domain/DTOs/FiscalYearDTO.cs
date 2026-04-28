using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;

namespace SoftBook.Domain.DTOs
{
    public class FiscalYearDTO
    {
        public int FiscalYearId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? IsCurrent { get; set; }
    }
}
