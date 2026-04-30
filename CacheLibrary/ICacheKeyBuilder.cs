namespace CacheLibrary;

public interface ICacheKeyBuilder
{
    string Build(string ns, string producer, string route, IReadOnlyDictionary<string, string?>? variance = null);
}
