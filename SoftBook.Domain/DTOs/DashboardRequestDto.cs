using System;

namespace SoftBook.Domain.DTOs
{
    public class DashboardRequestDto
    {
        public int CompanyId { get; set; }
        public string? FilterType { get; set; } // TODAY / THIS_WEEK / THIS_MONTH / THIS_YEAR
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? BranchId { get; set; }
        public string? Granularity { get; set; }
        public string? ZoneType { get; set; } // ALL / SALES / PURCHASE
        public string[]? ExpensePartyTypes { get; set; }
        public string[]? ExpenseVTypes { get; set; } // default = ['Expense']
        public int? TopLimit { get; set; }
    }
}
