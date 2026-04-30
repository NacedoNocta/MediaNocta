namespace CacheLibrary;

public sealed class CachingOptions
{
    public const string SectionName = "Caching";

    public bool Enabled { get; set; } = true;

    public bool RepopulateOnBypass { get; set; } = true;
}
