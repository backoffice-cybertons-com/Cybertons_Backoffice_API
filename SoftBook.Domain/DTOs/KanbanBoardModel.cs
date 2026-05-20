using System.Text.Json.Serialization;

namespace SoftBook.Domain.DTOs
{
    public class KanbanBoardModel
    {
        [JsonPropertyName("boardId")]
        public long BoardId { get; set; }

        [JsonPropertyName("boardName")]
        public string BoardName { get; set; } = string.Empty;

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }
}
