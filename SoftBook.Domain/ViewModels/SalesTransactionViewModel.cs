using System.Collections.Generic;
using SoftBook.Domain.Entities;

namespace SoftBook.Domain.ViewModels
{
    public class SalesTransactionViewModel
    {
        public SalesTransactionViewModel()
        {
            Header = new SalePur();
            Lines = new List<SalePurTran>();
        }

        public SalePur Header { get; set; }
        public List<SalePurTran> Lines { get; set; }
    }
}
