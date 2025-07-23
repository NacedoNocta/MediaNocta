using System.Text.Json.Serialization;
using TechLibrary.Interfaces;

namespace TechLibrary;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ITechUpdate))]
[JsonSerializable(typeof(TechUpdate))]
[JsonSerializable(typeof(List<ITechUpdate>))]
[JsonSerializable(typeof(List<TechUpdate>))]
public partial class TechLibraryJsonContext : JsonSerializerContext;