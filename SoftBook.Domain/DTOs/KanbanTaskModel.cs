using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.DTOs
{
    public class KanbanTaskModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("kanId")]
        public string KanId { get; set; } = string.Empty;

        [JsonPropertyName("cardId")]
        public string CardId { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("priority")]
        public string Priority { get; set; } = "Medium";

        [JsonPropertyName("module")]
        public string Module { get; set; } = string.Empty;

        [JsonPropertyName("assignedUserId")]
        public long? AssignedUserId { get; set; }

        [JsonPropertyName("dueDate")]
        public string DueDate { get; set; } = string.Empty;

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; } = string.Empty;

        [JsonPropertyName("isPinned")]
        public bool IsPinned { get; set; }

        [JsonPropertyName("coverImage")]
        public string CoverImage { get; set; } = string.Empty;

        [JsonPropertyName("labels")]
        public JsonElement? Labels { get; set; }

        [JsonPropertyName("badge1")]
        public JsonElement? Badge1 { get; set; }

        [JsonPropertyName("botId")]
        public string BotId { get; set; } = string.Empty;

        [JsonPropertyName("userImages")]
        public JsonElement? UserImages { get; set; }

        [JsonPropertyName("eye")]
        public int Eye { get; set; }

        [JsonPropertyName("que")]
        public int Que { get; set; }

        [JsonPropertyName("clip")]
        public int Clip { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("attachmentPaths")]
        public string AttachmentPaths { get; set; } = string.Empty;

        [JsonPropertyName("attachmentNames")]
        public string AttachmentNames { get; set; } = string.Empty;

        [JsonPropertyName("totalChecklistCount")]
        public int TotalChecklistCount { get; set; }

        [JsonPropertyName("completedChecklistCount")]
        public int CompletedChecklistCount { get; set; }
    }
}
