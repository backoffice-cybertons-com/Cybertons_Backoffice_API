using System;

namespace SoftBook.Domain.DTOs
{
    public class OutstandingTermConfigModel
    {
        public long ConfigId { get; set; }
        public long CompanyId { get; set; }
        public int MinDays { get; set; }
        public int MaxDays { get; set; }

        public string? BgColor { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateOutstandingTermConfigRequest
    {
        public long? CompanyId { get; set; }
        public int? MinDays { get; set; }
        public int? MaxDays { get; set; }

        public string? BgColor { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateOutstandingTermConfigRequest
    {
        public long ConfigId { get; set; }
        public long? CompanyId { get; set; }
        public int? MinDays { get; set; }
        public int? MaxDays { get; set; }

        public string? BgColor { get; set; }
        public bool IsActive { get; set; }
    }
}

