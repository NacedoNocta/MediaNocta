using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CacheLibrary;

public static class CacheTelemetry
{
    public const string SourceName = "MediaNocta.Cache";

    public static readonly ActivitySource ActivitySource = new(SourceName);
    public static readonly Meter Meter = new(SourceName);

    public static readonly Counter<long> Hits = Meter.CreateCounter<long>("cache.hit", description: "Number of cache hits.");
    public static readonly Counter<long> Misses = Meter.CreateCounter<long>("cache.miss", description: "Number of cache misses.");
    public static readonly Counter<long> Evictions = Meter.CreateCounter<long>("cache.evict", description: "Number of cache entries evicted.");
    public static readonly Counter<long> Bypasses = Meter.CreateCounter<long>("cache.bypass", description: "Number of requests that bypassed the cache.");

    public static KeyValuePair<string, object?>[] Tags(string ns, string producer, string? route = null) =>
        route is null
            ? new KeyValuePair<string, object?>[] { new("namespace", ns), new("producer", producer) }
            : new KeyValuePair<string, object?>[] { new("namespace", ns), new("producer", producer), new("route", route) };

    public static void RecordEviction(string ns, string producer, string? route = null, int count = 1)
    {
        if (count <= 0) return;
        Evictions.Add(count, Tags(ns, producer, route));
    }

    public static void RecordBypass(string ns, string producer, string? route = null)
    {
        Bypasses.Add(1, Tags(ns, producer, route));
    }

    public static void RecordHit(string ns, string producer, string? route = null)
    {
        Hits.Add(1, Tags(ns, producer, route));
    }

    public static void RecordMiss(string ns, string producer, string? route = null)
    {
        Misses.Add(1, Tags(ns, producer, route));
    }
}
