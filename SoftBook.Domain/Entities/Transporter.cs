using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabTransporterMaster")]
    public class Transporter
    {
        [Key]
        public int TransporterId { get; set; }
        
        public string? TransporterName { get; set; }
        public string? GSTNumber { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
