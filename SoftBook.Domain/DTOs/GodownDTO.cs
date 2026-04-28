using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class GodownDTO
    {
        public int GodownId { get; set; }
        public int? WarehouseId { get; set; }
        public string? GodownName { get; set; }
    }
}
