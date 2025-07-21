using SharedLibrary;
using TechLibrary.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TechLibrary;

public sealed class TechUpdate : Activity, ITechUpdate
{
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; }

    [JsonPropertyName("updateType")]
    public string UpdateType { get; set; }

    [JsonIgnore]
    public override string ThemeColor => TextKeys.techThemeColor;

    [JsonPropertyName("activityType")]
    public override string ActivityType => TextKeys.techTypeKey;

    public TechUpdate(LocalizedText title, LocalizedText summary, LocalizedText content, string projectId,
        string updateType = "", string? imageUrl = null, List<string>? links = null)
        : base(title, summary, content, imageUrl, links)
    {
        ProjectId = projectId;
        UpdateType = updateType;
    }

    public TechUpdate(string title, string summary, string content, string projectId,
        string updateType = "", string? imageUrl = null, List<string>? links = null)
        : this(new LocalizedText(title), new LocalizedText(summary), new LocalizedText(content),
               projectId, updateType, imageUrl, links)
    {
    }

    public TechUpdate() : base()
    {
        ProjectId = string.Empty;
        UpdateType = string.Empty;
    }

    public static TechUpdate? FromJson(string json)
        => JsonSerializer.Deserialize(json, TechLibraryJsonContext.Default.TechUpdate);

    public new string ToJson()
        => JsonSerializer.Serialize(this, TechLibraryJsonContext.Default.TechUpdate);
}