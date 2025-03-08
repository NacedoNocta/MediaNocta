using System.Text.Json.Serialization;

namespace BlogLibrairy;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Author))]
[JsonSerializable(typeof(Blog))]
[JsonSerializable(typeof(Blog))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(List<Blog>))]
public partial class BlogLibrairyJsonContext : JsonSerializerContext;
