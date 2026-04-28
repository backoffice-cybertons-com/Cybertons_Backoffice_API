//using System;

//namespace SoftBook.Domain.DTOs
//{
//    public class SalePurchaseRegisterDatewiseDTO
//    {
//        public DateTime InvDate { get; set; }
//        public string InvType { get; set; }
//        public decimal TotalQty { get; set; }
//        public decimal AvgPrice { get; set; }
//        public decimal TaxableAmount { get; set; }
//        public decimal Igst { get; set; }
//        public decimal Cgst { get; set; }
//        public decimal Sgst { get; set; }
//        public decimal TaxAmount { get; set; }
//        public decimal TotalAmount { get; set; }
//        public decimal RoundOff { get; set; }
//        public decimal FinalAmount { get; set; }
//        public decimal AddPrice { get; set; }
//        public decimal AddAmount { get; set; }
//        public decimal ExpDiscount { get; set; }
//    }
//}

using System;

namespace SoftBook.Domain.DTOs
{
    public class SalePurchaseRegisterDatewiseDTO
    {
        public DateTime InvDate { get; set; }
        public string InvType { get; set; }
        public decimal Qty { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal IgstAmount { get; set; }
        public decimal CgstAmount { get; set; }
        public decimal SgstAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RoundOff { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal AddPrice { get; set; }
        public decimal AddAmount { get; set; }
        public decimal TotDiscount { get; set; }
    }
}

