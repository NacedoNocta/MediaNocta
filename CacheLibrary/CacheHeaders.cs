using Microsoft.AspNetCore.Http;

namespace CacheLibrary;

public static class CacheHeaders
{
    public const string SkipCache = "X-Skip-Cache";

    private static readonly HashSet<string> TruthyValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "true", "1", "yes"
    };

    public static bool IsSkipCacheRequested(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue(SkipCache, out var values)) return false;
        foreach (var v in values)
        {
            if (v is not null && TruthyValues.Contains(v)) return true;
        }
        return false;
    }
}
