using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class PaymentModeDTO
    {
        public int PaymentModeId { get; set; }
        public string? ModeName { get; set; }
        public bool IsDefault { get; set; }
    }
}
