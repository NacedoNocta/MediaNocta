using System.Text.Json.Serialization;

namespace ArtLibrairy;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Art))]
public partial class ArtLibrairyJsonContext : JsonSerializerContext;
