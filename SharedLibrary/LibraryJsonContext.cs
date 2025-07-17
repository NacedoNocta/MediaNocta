using System.Text.Json.Serialization;
using SharedLibrary.Interfaces;

namespace SharedLibrary;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(IActivity))]
[JsonSerializable(typeof(Activity))]
[JsonSerializable(typeof(LocalizedText))]
[JsonSerializable(typeof(List<IActivity>))]
public partial class SharedLibraryJsonContext : JsonSerializerContext;
