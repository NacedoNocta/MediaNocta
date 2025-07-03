using System.Text.Json.Serialization;

namespace ArtLibrary;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Art))]
public partial class ArtLibraryJsonContext : JsonSerializerContext;
