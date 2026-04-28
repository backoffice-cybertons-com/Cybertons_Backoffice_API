using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class UnitDTO
    {
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public string? UQC { get; set; }
    }
}
