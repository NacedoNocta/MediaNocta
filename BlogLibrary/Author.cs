using BlogLibrary.Interfaces;
using SharedLibrary;
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
        public LocalizedText? Biography { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("twitter")]
        public string? Twitter { get; set; }

        [JsonPropertyName("linkedIn")]
        public string? LinkedIn { get; set; }

        [JsonPropertyName("github")]
        public string? GitHub { get; set; }

        public Author(string name, LocalizedText? biography = null, string? imageUrl = null, 
            string? email = null, string? website = null, string? twitter = null, 
            string? linkedIn = null, string? github = null)
        {
            Name = name;
            Biography = biography;
            ImageUrl = imageUrl;
            Email = email;
            Website = website;
            Twitter = twitter;
            LinkedIn = linkedIn;
            GitHub = github;
        }

        public Author(string name, string? biography = null, string? imageUrl = null, 
            string? email = null, string? website = null, string? twitter = null, 
            string? linkedIn = null, string? github = null)
            : this(name, biography != null ? new LocalizedText(biography) : null, imageUrl, 
                   email, website, twitter, linkedIn, github)
        {
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
