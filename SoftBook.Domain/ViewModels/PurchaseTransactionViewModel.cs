using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.ViewModels
{
    public class PurchaseTransactionViewModel
    {
        public PurchaseTransactionViewModel()
        {
            Header = new SalePur();
            Lines = new List<SalePurTran>();
        }

        public SalePur Header { get; set; }
        public List<SalePurTran> Lines { get; set; }
    }
}
