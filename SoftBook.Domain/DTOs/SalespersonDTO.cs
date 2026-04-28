using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class SalespersonDTO
    {
        public int SalespersonId { get; set; }
        public string? SalespersonName { get; set; }
        public string? Email { get; set; }
    }
}
