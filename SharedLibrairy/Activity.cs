using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLibrairy
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
        public string Title { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        [JsonPropertyName("links")]
        public List<string> Links { get; set; } = new();

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

        protected Activity(string title, string summary, string content, string? imageUrl = null, List<string>? links = null)
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

        public string ToJson()
            => JsonSerializer.Serialize(this, SharedLibrairyJsonContext.Default.Activity);
    }
}


