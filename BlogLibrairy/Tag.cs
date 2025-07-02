using BlogLibrairy.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogLibrairy
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

        private Tag() { }

        public Tag(string name)
        {
            Name = name;
        }

        public static Tag? FromJson(string json)
            => JsonSerializer.Deserialize(json, BlogLibrairyJsonContext.Default.Tag);

        public string ToJson()
            => JsonSerializer.Serialize(this, BlogLibrairyJsonContext.Default.Tag);
    }
}
