using System.Text.Json.Serialization;
using BlogLibrary.Interfaces;

namespace BlogLibrary;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Author))]
[JsonSerializable(typeof(Blog))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(ContentType))]
[JsonSerializable(typeof(BlogState))]
[JsonSerializable(typeof(List<Author>))]
[JsonSerializable(typeof(List<Blog>))]
public partial class BlogLibraryJsonContext : JsonSerializerContext;
