using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class SalePurchaseRegisterMonthwiseDTO
    {
        public string PartyName { get; set; }
        public string InvNo { get; set; }
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
