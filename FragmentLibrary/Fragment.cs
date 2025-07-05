using System.Text.Json.Serialization;
using SharedLibrary;
using FragmentLibrary.Interfaces;

namespace FragmentLibrary
{
    public class Fragment : Activity, IFragment
    {
        [JsonPropertyName("type_tag")]
        public string? TypeTag { get; set; }

        [JsonPropertyName("attachment_url")]
        public string? AttachmentUrl { get; set; }

        [JsonPropertyName("attachment_type")]
        public string? AttachmentType { get; set; }

        [JsonPropertyName("vibe_count")]
        public int VibeCount { get; set; }

        [JsonPropertyName("is_public")]
        public bool IsPublic { get; set; } = true;

        [JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [JsonPropertyName("meta")]
        public Dictionary<string, string>? Meta { get; set; }

        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        [JsonPropertyName("last_vibe_date")]
        public DateTime? LastVibeDate { get; set; }

        public override string ActivityType => TextKeys.fragmentTypeKey;

        public override string ThemeColor => TextKeys.fragmentThemeColor;

        public Fragment() : base() { }

        public Fragment(string title, string summary, string content, string? imageUrl = null, List<string>? links = null)
            : base(title, summary, content, imageUrl, links)
        {
        }
    }
}