using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.DTOs
{
    public class TaskTemplateModel
    {
        [JsonPropertyName("template_id")]
        public long TemplateId { get; set; }

        [JsonPropertyName("template_name")]
        public string TemplateName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("default_priority")]
        public string DefaultPriority { get; set; }

        [JsonPropertyName("default_status")]
        public string DefaultStatus { get; set; }

        [JsonPropertyName("default_assigned_user_id")]
        public long? DefaultAssignedUserId { get; set; }

        [JsonPropertyName("template_color")]
        public string? TemplateColor { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; }
        [JsonPropertyName("company_id")]
        public long CompanyId { get; set; }
    }
}
