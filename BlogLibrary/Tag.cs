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

        [SetsRequiredMembers]
        public Tag(string name)
        {
            Name = name;
        }
        private Tag() { }
        

        public static Tag? FromJson(string json)
            => JsonSerializer.Deserialize(json, BlogLibraryJsonContext.Default.Tag);

        public string ToJson()
            => JsonSerializer.Serialize(this, BlogLibraryJsonContext.Default.Tag);
    }
}
