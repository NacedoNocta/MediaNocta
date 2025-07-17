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

    [JsonPropertyName("contentType")]
    public ContentType ContentType { get; set; } = ContentType.Markdown;

    [JsonPropertyName("state")]
    public BlogState State { get; set; } = BlogState.Draft;

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("publishedAt")]
    public DateTime? PublishedAt { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("readingTimeMinutes")]
    public int ReadingTimeMinutes { get; set; } = 0;

    [JsonPropertyName("views")]
    public int Views { get; set; } = 0;

    [JsonPropertyName("activityType")]
    public override string ActivityType => TextKeys.blogTypeKey;

    public Blog(LocalizedText title, LocalizedText summary, LocalizedText content, Author author, 
        List<Tag>? tags = null, string? imageUrl = null, List<string>? links = null, 
        ContentType contentType = ContentType.Markdown, BlogState state = BlogState.Draft, 
        string? slug = null) : base(title, summary, content, imageUrl, links)
    {
        Author = author;
        ContentType = contentType;
        State = state;
        Slug = slug ?? GenerateSlug(title.English);
        if (tags != null)
        {
            Tags = tags;
        }
    }

    public Blog(string title, string summary, string content, Author author, List<Tag>? tags = null, 
        string? imageUrl = null, List<string>? links = null, ContentType contentType = ContentType.Markdown, 
        BlogState state = BlogState.Draft, string? slug = null)
        : this(new LocalizedText(title), new LocalizedText(summary), new LocalizedText(content), 
               author, tags, imageUrl, links, contentType, state, slug)
    {
    }

    public Blog() : base()
    {
        Author = new Author("", (string?)null);
        Slug = string.Empty;
    }

    public static Blog? FromJson(string json)
        => JsonSerializer.Deserialize(json, BlogLibraryJsonContext.Default.Blog);

    public new string ToJson()
        => JsonSerializer.Serialize(this, BlogLibraryJsonContext.Default.Blog);

    /// <summary>
    /// Generates a URL-friendly slug from the title
    /// </summary>
    /// <param name="title">The title to generate a slug from</param>
    /// <returns>A URL-friendly slug</returns>
    private static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        return title
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Trim('-');
    }

    /// <summary>
    /// Updates the published timestamp when the blog state changes to Published
    /// </summary>
    public void SetPublished()
    {
        State = BlogState.Published;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the updated timestamp
    /// </summary>
    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Estimates reading time based on content length (average 200 words per minute)
    /// </summary>
    /// <param name="language">Language to calculate reading time for</param>
    public void CalculateReadingTime(string language = "en")
    {
        var content = Content.GetText(language);
        var wordCount = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        ReadingTimeMinutes = Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));
    }
}


