using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class OutstandingComparisonDTO
    {
        public int PartyId { get; set; }
        public string CustomerName { get; set; }

        public decimal YQty { get; set; }
        public decimal YAmount { get; set; }
        public decimal YOs { get; set; }

        public decimal TQty { get; set; }
        public decimal TAmount { get; set; }
        public decimal Receipt { get; set; }

        public decimal TOs { get; set; }
        public decimal Diff { get; set; }
    }
}
