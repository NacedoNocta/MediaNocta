using BlogLibrary.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogLibrary
{
    public sealed class Author : IAuthor
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; } = Guid.NewGuid();

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("bio")]
        public string? Biography { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        public Author(string name, string? biography = null, string? imageUrl = null)
        {
            Name = name;
            Biography = biography;
            ImageUrl = imageUrl;
        }

        public Author()
        {
            Name = string.Empty;
        }

        public static Author? FromJson(string json)
            => JsonSerializer.Deserialize(json, BlogLibraryJsonContext.Default.Author);

        public string ToJson()
            => JsonSerializer.Serialize(this, BlogLibraryJsonContext.Default.Author);
    }
}
