using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SoftBook.Domain.DTOs
{
    public class KanbanColumnModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; } = "primary";

        [JsonPropertyName("badge")]
        public int Badge { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("cards")]
        public List<KanbanTaskModel> Cards { get; set; } = new List<KanbanTaskModel>();
    }
}
