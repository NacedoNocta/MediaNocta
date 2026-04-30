using System.Text;
using CacheLibrary;
using StackExchange.Redis;

namespace CacheAdminApi.Services;

public sealed class RedisInspector : IRedisInspector
{
    private readonly IConnectionMultiplexer _connection;
    private readonly ILogger<RedisInspector> _logger;

    public RedisInspector(IConnectionMultiplexer connection, ILogger<RedisInspector> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CacheEntryDto>> GetEntriesAsync(string? @namespace, string? producer, string? routePrefix, int page, int pageSize, CancellationToken ct)
    {
        var pattern = BuildPattern(@namespace, producer, routePrefix);
        var entries = new List<CacheEntryDto>();
        var skip = (page - 1) * pageSize;
        var taken = 0;
        var seen = 0;

        await foreach (var entry in EnumerateAsync(pattern, ct))
        {
            if (seen++ < skip) continue;
            entries.Add(entry);
            if (++taken >= pageSize) break;
        }

        return entries;
    }

    public async Task<int> CountEntriesAsync(string? @namespace, string? producer, string? routePrefix, CancellationToken ct)
    {
        var pattern = BuildPattern(@namespace, producer, routePrefix);
        var count = 0;
        await foreach (var _ in EnumerateAsync(pattern, ct, metadata: false)) count++;
        return count;
    }

    public async Task<IReadOnlyList<CacheGroupSummaryDto>> GetGroupsAsync(CancellationToken ct)
    {
        var groups = new Dictionary<(string Ns, string Producer), GroupAccumulator>();

        var producers = new (string Namespace, string Producer)[]
        {
            (CacheNamespaces.Main, CacheProducers.BlogApi),
            (CacheNamespaces.Main, CacheProducers.ActivityApi),
            (CacheNamespaces.Main, CacheProducers.FragmentApi),
            (CacheNamespaces.Main, CacheProducers.TechApi),
            (CacheNamespaces.Main, CacheProducers.Gateway),
            (CacheNamespaces.Website, CacheProducers.WebsiteClient)
        };
        foreach (var p in producers) groups[p] = new GroupAccumulator();

        await foreach (var entry in EnumerateAsync($"{CacheKeys.SolutionPrefix}:*", ct))
        {
            var key = (entry.Namespace, entry.Producer);
            if (!groups.TryGetValue(key, out var acc))
            {
                acc = new GroupAccumulator();
                groups[key] = acc;
            }
            acc.Add(entry.WrittenAtUtc, entry.TtlRemainingSeconds);
        }

        return groups.Select(kv => new CacheGroupSummaryDto(
            kv.Key.Ns,
            kv.Key.Producer,
            kv.Value.Count,
            kv.Value.Oldest,
            kv.Value.Newest,
            kv.Value.ShortestTtlRemaining,
            kv.Value.LongestTtlRemaining)).ToList();
    }

    public async Task<bool> EvictAsync(string key, CancellationToken ct)
    {
        var db = _connection.GetDatabase();
        var deleted = await db.KeyDeleteAsync(key);
        if (deleted)
        {
            CacheTelemetry.Evictions.Add(1, new KeyValuePair<string, object?>("namespace", "explicit"), new KeyValuePair<string, object?>("producer", "cache-admin"));
        }
        return deleted;
    }

    public async Task<int> EvictByPrefixAsync(string prefix, CancellationToken ct)
    {
        var pattern = prefix.EndsWith('*') ? prefix : prefix + "*";
        var count = 0;
        var db = _connection.GetDatabase();
        var endpoints = _connection.GetEndPoints();

        foreach (var endpoint in endpoints)
        {
            var server = _connection.GetServer(endpoint);
            if (!server.IsConnected || server.IsReplica) continue;

            await foreach (var key in server.KeysAsync(pattern: pattern).WithCancellation(ct))
            {
                if (await db.KeyDeleteAsync(key)) count++;
            }
        }

        if (count > 0)
        {
            CacheTelemetry.Evictions.Add(count, new KeyValuePair<string, object?>("namespace", "explicit"), new KeyValuePair<string, object?>("producer", "cache-admin"));
        }
        return count;
    }

    private static string BuildPattern(string? @namespace, string? producer, string? routePrefix)
    {
        var sb = new StringBuilder(CacheKeys.SolutionPrefix).Append(':');
        sb.Append(string.IsNullOrEmpty(@namespace) ? "*" : @namespace).Append(':');
        sb.Append(string.IsNullOrEmpty(producer) ? "*" : producer).Append(':');
        sb.Append(string.IsNullOrEmpty(routePrefix) ? "*" : routePrefix + "*");
        return sb.ToString();
    }

    private async IAsyncEnumerable<CacheEntryDto> EnumerateAsync(string pattern, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct, bool metadata = true)
    {
        var db = _connection.GetDatabase();
        foreach (var endpoint in _connection.GetEndPoints())
        {
            var server = _connection.GetServer(endpoint);
            if (!server.IsConnected || server.IsReplica) continue;

            await foreach (var redisKey in server.KeysAsync(pattern: pattern).WithCancellation(ct))
            {
                var keyStr = redisKey.ToString();
                if (!metadata)
                {
                    yield return new CacheEntryDto(keyStr, string.Empty, "", "", "", "", DateTimeOffset.MinValue, null, null);
                    continue;
                }

                var ttl = await db.KeyTimeToLiveAsync(redisKey);
                long? ttlSeconds = ttl.HasValue ? (long)ttl.Value.TotalSeconds : null;

                yield return ToDto(keyStr, ttlSeconds);
            }
        }
    }

    private static CacheEntryDto ToDto(string key, long? ttlRemainingSeconds)
    {
        // Key format: mn:{namespace}:{producer}:{route}[:variance]
        var parts = key.Split(':', 5);
        var ns = parts.Length > 1 ? parts[1] : "";
        var producer = parts.Length > 2 ? parts[2] : "";
        var route = parts.Length > 3 ? parts[3] : "";
        var variance = parts.Length > 4 ? parts[4].Replace(':', ' ') : "";

        // Aspire OutputCache stores entries with their full TTL configured at creation; we don't know the original
        // configured TTL from Redis alone, so written-at is best-approximated as "now" for ttl-bearing keys.
        var writtenAt = DateTimeOffset.UtcNow;

        return new CacheEntryDto(
            key,
            Convert.ToBase64String(Encoding.UTF8.GetBytes(key)).Replace('+', '-').Replace('/', '_').TrimEnd('='),
            ns,
            producer,
            route,
            variance,
            writtenAt,
            ttlRemainingSeconds,
            ttlRemainingSeconds);
    }

    private sealed class GroupAccumulator
    {
        public int Count;
        public DateTimeOffset? Oldest;
        public DateTimeOffset? Newest;
        public long? ShortestTtlRemaining;
        public long? LongestTtlRemaining;

        public void Add(DateTimeOffset writtenAt, long? ttlRemaining)
        {
            Count++;
            if (Oldest is null || writtenAt < Oldest) Oldest = writtenAt;
            if (Newest is null || writtenAt > Newest) Newest = writtenAt;
            if (ttlRemaining is not null)
            {
                if (ShortestTtlRemaining is null || ttlRemaining < ShortestTtlRemaining) ShortestTtlRemaining = ttlRemaining;
                if (LongestTtlRemaining is null || ttlRemaining > LongestTtlRemaining) LongestTtlRemaining = ttlRemaining;
            }
        }
    }
}
