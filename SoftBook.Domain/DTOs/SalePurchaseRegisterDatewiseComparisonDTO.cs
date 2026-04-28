using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class SalePurchaseRegisterDatewiseComparisonDTO
    {
        public DateTime? PInvDate { get; set; }
        public DateTime? SInvDate { get; set; }

        public decimal PurQty { get; set; }
        public decimal PurAvgPrice { get; set; }
        public decimal PurAmount { get; set; }
        public decimal PurTax { get; set; }
        public decimal PurExpDisRound { get; set; }
        public decimal PurFinal { get; set; }

        public decimal SaleQty { get; set; }
        public decimal SaleAvgPrice { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal SaleTax { get; set; }
        public decimal SaleExpDisRound { get; set; }
        public decimal SaleFinal { get; set; }

        public decimal PL { get; set; }

       
    }
}
