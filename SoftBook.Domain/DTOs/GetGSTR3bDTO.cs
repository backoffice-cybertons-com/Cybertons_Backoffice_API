using System;
using System.Collections.Generic;
using System.Text;

namespace SoftBook.Domain.DTOs
{
    public class GetGSTR3bDTO
    {
        public string Section { get; set; } = string.Empty;

        public decimal TaxableAmt { get; set; }

        public decimal IgstAmt { get; set; }

        public decimal CgstAmt { get; set; }

        public decimal SgstAmt { get; set; }

        public decimal Cess { get; set; }

        public decimal InterStateSupplies { get; set; }

        public decimal IntraStateSupplies { get; set; }
    }
}
