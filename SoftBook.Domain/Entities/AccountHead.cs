using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("TabAccountMaster")]
    public class AccountHead
    {
        [Key]
        public int AccountId { get; set; }
        
        public string? AccountType { get; set; }
        public string? AccountName { get; set; }
        public bool? IsSubAccount { get; set; }
        public int? ParentAccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? Description { get; set; }
    }
}
