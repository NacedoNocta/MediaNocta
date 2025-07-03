using BlogLibrary.Interfaces;
using SharedLibrary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogLibrary;

public sealed class Blog : Activity, IBlog
{

    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = new();

    [JsonIgnore]
    public override string ThemeColor => TextKeys.blogThemeColor;

    [JsonPropertyName("author")]
    public Author Author { get; set; }

    [JsonPropertyName("activityType")]
    public override string ActivityType => TextKeys.blogTypeKey;

    public Blog(string title, string summary, string content, Author author, List<Tag>? tags = null, string? imageUrl = null, List<string>? links = null)
        : base(title, summary, content, imageUrl, links)
    {
        Author = author;
        if (tags != null)
        {
            Tags = tags;
        }
    }

    public Blog() : base("", "", "")
    {
        Author = new Author("");
    }

    public static Blog? FromJson(string json)
        => JsonSerializer.Deserialize(json, BlogLibraryJsonContext.Default.Blog);

    public new string ToJson()
        => JsonSerializer.Serialize(this, BlogLibraryJsonContext.Default.Blog);
}
