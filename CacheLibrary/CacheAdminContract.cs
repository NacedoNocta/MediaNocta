namespace CacheLibrary;

public sealed record CacheEntryDto(
    string Key,
    string KeyBase64,
    string Namespace,
    string Producer,
    string Route,
    string VarianceLabel,
    DateTimeOffset WrittenAtUtc,
    long? TtlSeconds,
    long? TtlRemainingSeconds);

public sealed record CacheGroupSummaryDto(
    string Namespace,
    string Producer,
    int EntryCount,
    DateTimeOffset? OldestWrittenAtUtc,
    DateTimeOffset? NewestWrittenAtUtc,
    long? ShortestTtlRemainingSeconds,
    long? LongestTtlRemainingSeconds);

public sealed record CacheEntriesResponse(
    IReadOnlyList<CacheEntryDto> Entries,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record CacheGroupsResponse(
    IReadOnlyList<CacheGroupSummaryDto> Groups);

public sealed record CacheEvictionResponse(int EvictedCount);
