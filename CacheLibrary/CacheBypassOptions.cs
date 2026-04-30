namespace CacheLibrary;

public sealed class CacheBypassOptions
{
    public string Namespace { get; set; } = CacheNamespaces.Main;
    public string Producer { get; set; } = CacheProducers.Gateway;
}
