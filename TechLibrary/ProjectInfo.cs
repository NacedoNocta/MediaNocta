using SharedLibrary;

namespace TechLibrary;

public class ProjectInfo
{
    public ProjectKeys Key { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
}
