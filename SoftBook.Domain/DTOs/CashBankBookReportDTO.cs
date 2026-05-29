using System;

namespace SoftBook.Domain.DTOs
{
    // Code developed by Hetak Modi 290526
    public class CashBankBookReportDTO
    {
        public DateTime TransactionDate { get; set; }
        public long? VoucherNo { get; set; }
        public string VoucherType { get; set; }
        public string Particulars { get; set; }
        public decimal ReceiptAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal Balance { get; set; }
        public string? BankName { get; set; }
    }
}
