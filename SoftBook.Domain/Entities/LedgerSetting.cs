using System;

namespace SoftBook.Domain.Entities
{
    public class LedgerSetting
    {
        public int SettingId { get; set; }
        public int CompanyId { get; set; }
        public bool AutoNumbering { get; set; } = true;
        public string? DefaultCurrency { get; set; }
        public bool EnableTcs { get; set; } = false;
        public bool EnableTds { get; set; } = false;

        // New Account Mappings
        public int? SalesRevenueAccountId { get; set; }
        public int? PurchaseExpenseAccountId { get; set; }
        public int? CgstOutputAccountId { get; set; }
        public int? SgstOutputAccountId { get; set; }
        public int? IgstOutputAccountId { get; set; }
        public int? CgstInputAccountId { get; set; }
        public int? SgstInputAccountId { get; set; }
        public int? IgstInputAccountId { get; set; }
        public int? RoundOffAccountId { get; set; }
        public long? DiscountAccountId { get; set; }
    }
}
