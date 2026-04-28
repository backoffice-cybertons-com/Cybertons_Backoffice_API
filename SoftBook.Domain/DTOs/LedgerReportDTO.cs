//using System;
//using System.Collections.Generic;

//namespace SoftBook.Domain.DTOs
//{
//    public class LedgerEntryDTO
//    {
//        public DateTime JvDate { get; set; }
//        public string? VType { get; set; }
//        public long? VCode { get; set; }
//        public string? Description { get; set; }
//        public decimal Debit { get; set; }
//        public decimal Credit { get; set; }
//        public decimal Balance { get; set; }
//        public long? Id { get; set; }
//    }

//    public class LedgerReportDTO
//    {
//        public int AccountCode { get; set; }
//        public decimal OpeningBalance { get; set; }
//        public decimal ClosingBalance { get; set; }
//        public List<LedgerEntryDTO> Entries { get; set; } = new List<LedgerEntryDTO>();
//    }

//	//InvWise
//	public class InvWiseDTO
//	{
//		public DateTime InvDate { get; set; }
//		public string? InvNo { get; set; }
//		public string? InvType { get; set; }
//		public string? CustomerName { get; set; }
//		public decimal Qty { get; set; }
//		public int SpUniqueCode { get; set; }

//		public decimal TaxableAmount { get; set; }
//		public decimal AvgPrice { get; set; }
//		public decimal IGST { get; set; }
//		public decimal CGST { get; set; }
//		public decimal SGST { get; set; }
//		public decimal TaxAmount { get; set; }
//		public decimal TotalAmount { get; set; }
//		public decimal ExpDiscount { get; set; }
//		public decimal RoundOff { get; set; }
//		public decimal FinalAmount { get; set; }
//		public decimal AddPrice { get; set; }
//		public decimal AddAmount { get; set; }
//	}
//}


using System;
using System.Collections.Generic;

namespace SoftBook.Domain.DTOs
{
    public class LedgerEntryDTO
    {
        public DateTime JvDate { get; set; }
        public string? VType { get; set; }
        public long? VCode { get; set; }
        public string? Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public long? Id { get; set; }
    }

    public class LedgerReportDTO
    {
        public int AccountCode { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<LedgerEntryDTO> Entries { get; set; } = new List<LedgerEntryDTO>();
    }

    //InvWise
    public class InvWiseDTO
    {
        public DateTime InvDate { get; set; }
        public string? InvNo { get; set; }
        public string? InvType { get; set; }
        public string? PartyName { get; set; }
        public decimal Qty { get; set; }
        public int SpUniqueCode { get; set; }

        public decimal TaxableAmount { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal IgstAmount { get; set; }
        public decimal CgstAmount { get; set; }
        public decimal SgstAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotDiscount { get; set; }
        public decimal RoundOff { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal AddPrice { get; set; }
        public decimal AddAmount { get; set; }
    }
}

