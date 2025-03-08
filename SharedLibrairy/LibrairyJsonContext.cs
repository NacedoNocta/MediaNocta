using System.Text.Json.Serialization;

namespace SharedLibrairy;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(IActivity))]
[JsonSerializable(typeof(List<IActivity>))]
public partial class SharedLibrairyJsonContext : JsonSerializerContext;
