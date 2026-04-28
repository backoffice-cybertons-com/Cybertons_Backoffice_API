using System.Collections.Generic;

namespace SoftBook.Domain.DTOs
{
    public class RolePermissionUpdateDTO
    {
        public int RoleId { get; set; }
        public List<RolePermissionItemDTO> Permissions { get; set; } = new List<RolePermissionItemDTO>();
    }

    public class RolePermissionItemDTO
    {
        public int TabId { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
