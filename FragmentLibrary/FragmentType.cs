using System.Text.Json.Serialization;

namespace FragmentLibrary
{
    public class FragmentType
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; } = Guid.NewGuid();

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public FragmentType() { }

        public FragmentType(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }
    
    public static class FragmentTypes
    {
        public static readonly FragmentType[] DefaultTypes = 
        {
            new() { Name = "idea", Color = "warning" },      
            new() { Name = "code", Color = "info" },         
            new() { Name = "quote", Color = "secondary" },   
            new() { Name = "glitch", Color = "danger" },     
            new() { Name = "dream", Color = "primary" },     
            new() { Name = "fragment", Color = "success" },  
            new() { Name = "fiction", Color = "dark" }       
        };
    }
}