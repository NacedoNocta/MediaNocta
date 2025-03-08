using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogLibrairy
{
    public sealed class Tag
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; } = Guid.NewGuid();

        [JsonPropertyName("name")]
        public required string Name { get; set; }

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
