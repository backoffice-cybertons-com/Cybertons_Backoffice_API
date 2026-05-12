using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.DTOs
{
    public class TaskModel
    {
        [JsonPropertyName("task_id")]
        public long TaskId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("initiated_user_id")]
        public long InitiatedUserId { get; set; }

        [JsonPropertyName("assigned_user_id")]
        public long? AssignedUserId { get; set; }

        [JsonPropertyName("assigned_date")]
        public DateTime? AssignedDate { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("expected_date")]
        public DateTime? ExpectedDate { get; set; }

        [JsonPropertyName("reminder")]
        public bool Reminder { get; set; }

        [JsonPropertyName("reminder_date")]
        public DateTime? ReminderDate { get; set; }

        [JsonPropertyName("finish_date")]
        public DateTime? FinishDate { get; set; }

        [JsonPropertyName("finish_description")]
        public string? FinishDescription { get; set; }

        [JsonPropertyName("task_status")]
        public string TaskStatus { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("task_category_id")]
        public long? TaskCategoryId { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; }
        [JsonPropertyName("company_id")]
        public long CompanyId { get; set; }
    }
}
