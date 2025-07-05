using System.Text.Json.Serialization;

namespace FragmentLibrary
{
    [JsonSerializable(typeof(Fragment))]
    [JsonSerializable(typeof(FragmentType))]
    [JsonSerializable(typeof(IEnumerable<Fragment>))]
    [JsonSerializable(typeof(IEnumerable<FragmentType>))]
    public partial class FragmentLibraryJsonContext : JsonSerializerContext
    {
    }
}