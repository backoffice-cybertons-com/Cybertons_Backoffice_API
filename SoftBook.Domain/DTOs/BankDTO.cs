using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class BankDTO
    {
        public int BankMasterId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string? Branch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSC { get; set; }
        public string? MICR { get; set; }
    }
}
