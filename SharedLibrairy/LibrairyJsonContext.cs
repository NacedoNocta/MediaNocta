using System.Text.Json.Serialization;
using SharedLibrairy.Interfaces;

namespace SharedLibrairy;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(IActivity))]
[JsonSerializable(typeof(Activity))]
[JsonSerializable(typeof(List<IActivity>))]
public partial class SharedLibrairyJsonContext : JsonSerializerContext;
