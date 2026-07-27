using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabZoneMaster")]
    public class Zone
    {
        [Key]
        public int ZoneId { get; set; }

        [Required]
        public string ZoneName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
