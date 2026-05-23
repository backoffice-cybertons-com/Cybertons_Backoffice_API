using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class DailyOutstandingDto
    {
        public long PartyId { get; set; }

        public string PartyName { get; set; }

        public DateTime ReportDate { get; set; }

        public decimal Outstanding { get; set; }
    }
}
