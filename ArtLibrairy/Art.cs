using SharedLibrairy;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArtLibrairy;

public sealed class Art : IActivity
{

    [JsonIgnore]
    public override string ThemeColor => "success";

    [JsonPropertyName("activityType")]
    public override string ActivityType => TextKeys.artTypeKey;

    public Art(string title, string summary, string content, string? imageUrl = null, List<string>? links = null)
        : base(title, summary, content, imageUrl, links)
    {

    }

    public static Art? FromJson(string json)
        => JsonSerializer.Deserialize(json, ArtLibrairyJsonContext.Default.Art);

    public new string ToJson()
        => JsonSerializer.Serialize(this, ArtLibrairyJsonContext.Default.Art);
}
