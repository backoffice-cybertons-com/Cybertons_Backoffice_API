using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
namespace SoftBook.Domain.Entities
{
    public class SidebarItem
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int TabId { get; set; }
        public string TabName { get; set; }
        public string TabUrl { get; set; }
        public int? ParentId { get; set; }
        public string IconPath { get; set; }
        public int SortOrder { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

        public List<SidebarItem> Children { get; set; } = new List<SidebarItem>();
    }
}
