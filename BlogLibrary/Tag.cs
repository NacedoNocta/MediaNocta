using BlogLibrary.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogLibrary
{

    [Table("Tags")]
    public sealed class Tag : ITag
    {
        [JsonPropertyName("id")]
        [Key]
        public Guid Id { get; init; } = Guid.NewGuid();

        [JsonPropertyName("name")]
        [Required]
        public required string Name { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; } = "secondary";

        [SetsRequiredMembers]
        public Tag(string name, string color = "secondary")
        {
            Name = name;
            Color = color;
        }
        private Tag() { }
        

        public static Tag? FromJson(string json)
            => JsonSerializer.Deserialize(json, BlogLibraryJsonContext.Default.Tag);

        public string ToJson()
            => JsonSerializer.Serialize(this, BlogLibraryJsonContext.Default.Tag);
    }
}
