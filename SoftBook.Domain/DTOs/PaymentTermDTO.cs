using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class PaymentTermDTO
    {
        public int TermId { get; set; }
        public string? TermName { get; set; }
        public int? NumberOfDays { get; set; }
    }
}
