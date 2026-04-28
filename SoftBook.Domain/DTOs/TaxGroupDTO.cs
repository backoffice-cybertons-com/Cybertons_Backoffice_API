using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class TaxGroupDTO
    {
        public int TaxGroupId { get; set; }
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public string? SupplyType { get; set; }
    }
}
