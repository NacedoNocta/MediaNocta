using SharedLibrary;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArtLibrary.Interfaces;
using SharedLibrary.Interfaces;

namespace ArtLibrary;

public sealed class Art : Activity, IArt
{

    [JsonIgnore]
    public override string ThemeColor => TextKeys.artThemeColor;

    [JsonPropertyName("activityType")]
    public override string ActivityType => TextKeys.artTypeKey;

    public Art(string title, string summary, string content, string? imageUrl = null, List<string>? links = null)
        : base(title, summary, content, imageUrl, links)
    {

    }

    public static Art? FromJson(string json)
        => JsonSerializer.Deserialize(json, ArtLibraryJsonContext.Default.Art);

    public new string ToJson()
        => JsonSerializer.Serialize(this, ArtLibraryJsonContext.Default.Art);
}
