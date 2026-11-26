using System;

namespace SoftBook.Domain.DTOs
{
    public class CashBankPartyDetailDto
    {
        public long PartyId { get; set; }
        public string? PartyName { get; set; }
        public string? AccountGroup { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class CashBankPartyDetailsRequestDto
    {
        public int CompanyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? AccountGroup { get; set; }
    }
}
