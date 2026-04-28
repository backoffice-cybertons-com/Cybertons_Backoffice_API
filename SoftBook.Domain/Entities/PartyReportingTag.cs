using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabPartyReportingTags")]
    public class PartyReportingTag
    {
        [Key]
        public int PartyReportingTagId { get; set; }

        public int PartyId { get; set; }

        [Required]
        public int ReportingTagId { get; set; }

        public string? TagValue { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}




