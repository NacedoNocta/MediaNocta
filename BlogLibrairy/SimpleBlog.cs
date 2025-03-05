using System.Text.Json.Serialization;

namespace BlogLibrairy;

public class SimpleBlog
{
    [JsonPropertyName("Index")]
    public int Index { get; set; }

    [JsonPropertyName("Title")]
    public string Title { get; set; }

    [JsonPropertyName("Content")]
    public string Content { get; set; }

    [JsonPropertyName("Author")]
    public string Author { get; set; }

    [JsonPropertyName("Tags")]
    public string? Tags { get; set; }

}

[JsonSerializable(typeof(List<SimpleBlog>))]
public sealed partial class SimpleBlogSerializerContext : JsonSerializerContext
{
}
