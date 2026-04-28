using System;

namespace SoftBook.Domain.Entities
{
    public class JournalVoucher
    {
        public long JvCode { get; set; }
        public DateTime JvDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        
        public long DrAccountCode { get; set; }
        public string? DrAccountName { get; set; }
        public long CrAccountCode { get; set; }
        public string? CrAccountName { get; set; }
        public decimal Amount { get; set; } = 0;
        
        public string? VType { get; set; }
        public long? VCode { get; set; }
        public string? Description { get; set; }
        public string? AdjustVType { get; set; }
        
        public int JvCompCode { get; set; }
        public string? GCode { get; set; }
        public string? ChequeNo { get; set; }
        public int? UserCode { get; set; }
        public int? GroupJvSrNo { get; set; }
        
        public bool IsOutstanding { get; set; } = false;
        public int YearCode { get; set; }
        public int BranchCode { get; set; }
        
        public bool DeleteFlag { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
