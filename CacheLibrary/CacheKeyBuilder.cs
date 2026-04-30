namespace CacheLibrary;

public sealed class CacheKeyBuilder : ICacheKeyBuilder
{
    public string Build(string ns, string producer, string route, IReadOnlyDictionary<string, string?>? variance = null)
    {
        var canonicalRoute = (route ?? string.Empty).Trim('/').ToLowerInvariant();
        var varianceSegment = BuildVariance(variance);
        return varianceSegment.Length == 0
            ? $"{CacheKeys.SolutionPrefix}:{ns}:{producer}:{canonicalRoute}"
            : $"{CacheKeys.SolutionPrefix}:{ns}:{producer}:{canonicalRoute}:{varianceSegment}";
    }

    private static string BuildVariance(IReadOnlyDictionary<string, string?>? variance)
    {
        if (variance is null || variance.Count == 0) return string.Empty;

        return string.Join(":", variance
            .OrderBy(kv => kv.Key, StringComparer.Ordinal)
            .Select(kv => $"{kv.Key.ToLowerInvariant()}={kv.Value ?? string.Empty}"));
    }
}
