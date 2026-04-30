using CacheLibrary;

namespace CacheAdminApi.Services;

public interface IRedisInspector
{
    Task<IReadOnlyList<CacheEntryDto>> GetEntriesAsync(string? @namespace, string? producer, string? routePrefix, int page, int pageSize, CancellationToken ct);
    Task<int> CountEntriesAsync(string? @namespace, string? producer, string? routePrefix, CancellationToken ct);
    Task<IReadOnlyList<CacheGroupSummaryDto>> GetGroupsAsync(CancellationToken ct);
    Task<bool> EvictAsync(string key, CancellationToken ct);
    Task<int> EvictByPrefixAsync(string prefix, CancellationToken ct);
}
