using System.Text.Json.Serialization;
using SharedLibrary.Interfaces;

namespace BlogLibrary.Interfaces;

public interface IBlog : IActivity
{
    List<Tag> Tags { get; set; }
    Author Author { get; set; }
    ContentType ContentType { get; set; }
    BlogState State { get; set; }
    DateTime? UpdatedAt { get; set; }
    DateTime? PublishedAt { get; set; }
    string Slug { get; set; }
    int ReadingTimeMinutes { get; set; }
    int Views { get; set; }
    
    new string ToJson();
    void SetPublished();
    void MarkAsUpdated();
    void CalculateReadingTime(string language = "en");
}

[JsonConverter(typeof(JsonStringEnumConverter<BlogState>))]
public enum BlogState
{
    Draft,
    Private,
    Published
}
    
[JsonConverter(typeof(JsonStringEnumConverter<ContentType>))]
public enum ContentType
{
    Markdown,
    Html,
    PlainText
}