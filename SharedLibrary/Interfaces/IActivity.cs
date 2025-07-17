using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLibrary.Interfaces
{
    public interface IActivity
    {
        public Guid Id { get; init; }
        public LocalizedText Title { get; set; }
        public LocalizedText Summary { get; set; }
        public LocalizedText Content { get; set; }
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; init; }
        public List<string> Links { get; set; }
        public bool Featured { get; set; }
        
        public abstract string ActivityType { get; }

        public abstract string ThemeColor { get; }

        public string ToJson();
    }
}


