using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLibrary
{
    /// <summary>
    /// An activity, a broad element used to define all blogs posts, tech content, art content, etc. in a generic,
    /// undifferentiated form. 
    /// </summary>
    public abstract class Activity
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; } = Guid.NewGuid();

        [JsonPropertyName("title")]
        public LocalizedText Title { get; set; } = new();

        [JsonPropertyName("summary")]
        public LocalizedText Summary { get; set; } = new();

        [JsonPropertyName("content")]
        public LocalizedText Content { get; set; } = new();

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        [JsonPropertyName("links")]
        public List<string> Links { get; set; } = new();

        [JsonPropertyName("featured")]
        public bool Featured { get; set; } = false;

        /// <summary>
        /// The type of the activity, used to determine the class of the activity when 
        /// requesting any of them indifferently. All child of the same class have the same value.
        /// Possibles values must be listed in TextKeys.cs and used in the child classes. 
        /// </summary>
        [JsonPropertyName("activityType")]
        public abstract string ActivityType { get; }

        /// <summary>
        /// The color of the activity. All child of the same class have the same value. 
        /// Possible values : "primary", "secondary", "success", "danger", "warning", "info", "light", "dark".
        /// </summary>
        [JsonIgnore]
        public abstract string ThemeColor { get; }

        protected Activity(LocalizedText title, LocalizedText summary, LocalizedText content, string? imageUrl = null, List<string>? links = null)
        {
            Title = title;
            Summary = summary;
            Content = content;
            ImageUrl = imageUrl;
            if (links != null)
            {
                Links = links;
            }
        }

        protected Activity(string title, string summary, string content, string? imageUrl = null, List<string>? links = null)
            : this(new LocalizedText(title), new LocalizedText(summary), new LocalizedText(content), imageUrl, links)
        {
        }

        protected Activity()
        {
            Title = new LocalizedText();
            Summary = new LocalizedText();
            Content = new LocalizedText();
        }

        public string ToJson()
            => JsonSerializer.Serialize(this, SharedLibraryJsonContext.Default.Activity);
    }
}


