using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.DTOs
{
    public class TransporterDTO
    {
        public int TransporterId { get; set; }
        public string? TransporterName { get; set; }
        public string? GSTNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
