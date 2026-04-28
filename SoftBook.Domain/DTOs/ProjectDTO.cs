using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System;

namespace SoftBook.Domain.DTOs
{
    public class ProjectDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
    }
}
