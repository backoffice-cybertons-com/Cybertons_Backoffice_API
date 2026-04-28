using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftBook.Domain.Entities
{
    [Table("tab_role_permission")]
    public class RolePermission
    {
        [Key]
        public int PermissionId { get; set; }
        
        public int CompanyId { get; set; }
        
        public int RoleId { get; set; }
        
        public int TabId { get; set; }
        
        public bool CanView { get; set; }
        
        public bool CanAdd { get; set; }
        
        public bool CanEdit { get; set; }
        
        public bool CanDelete { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties if needed
        [NotMapped]
        public string? TabName { get; set; }
        
        [NotMapped]
        public string? TabUrl { get; set; }
    }
}
