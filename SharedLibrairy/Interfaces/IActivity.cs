using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLibrairy.Interfaces
{
    public interface IActivity
    {
        public Guid Id { get; init; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; init; }
        public List<string> Links { get; set; }
        
        public abstract string ActivityType { get; }

        public abstract string ThemeColor { get; }

        public string ToJson();
    }
}


