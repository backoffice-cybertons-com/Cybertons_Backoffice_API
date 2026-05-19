using System;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.DTOs
{
    public class KanbanChecklistItemModel
    {
        [JsonPropertyName("checklistItemId")]
        public long ChecklistItemId { get; set; }

        [JsonPropertyName("kanbanTaskId")]
        public long KanbanTaskId { get; set; }

        [JsonPropertyName("checklistGroup")]
        public string ChecklistGroup { get; set; } = string.Empty;

        [JsonPropertyName("itemText")]
        public string ItemText { get; set; } = string.Empty;

        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonPropertyName("completedDate")]
        public DateTime? CompletedDate { get; set; }

        [JsonPropertyName("positionOrder")]
        public int PositionOrder { get; set; }

        [JsonPropertyName("createdBy")]
        public long? CreatedBy { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("updatedBy")]
        public long? UpdatedBy { get; set; }

        [JsonPropertyName("updatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
