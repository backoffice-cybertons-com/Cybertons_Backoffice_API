using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class PayebleReceivableDTO
    {
        public int? PartyId { get; set; }
        public string DisplayName { get; set; }
        public string PartyType { get; set; }
        public string Contact { get; set; }

        public string OpeningBalanceType { get; set; }

        public decimal InvAmt { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal SalesAmt { get; set; }
        public decimal PaidAmt { get; set; }
        public decimal Outstanding { get; set; }

        public string CreditDebit { get; set; }

        public decimal Terms { get; set; }
        public decimal Weekly { get; set; }
        public decimal Monthly { get; set; }
        public decimal MonthlyTerm { get; set; }

        public string? Kam { get; set; }
        public string? RecoveryAgent { get; set; }
        public string? SumulIncharge { get; set; }
        public string? ZoneName { get; set; }
        public string? SalespersonName { get; set; }
        public string? TransporterName { get; set; }
    }
}
