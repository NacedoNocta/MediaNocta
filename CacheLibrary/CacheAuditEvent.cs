namespace CacheLibrary;

public enum CacheAuditAction
{
    EvictEntry,
    EvictGroup,
    FlushAll
}

public sealed record CacheAuditEvent(
    DateTimeOffset OccurredAtUtc,
    string ActorId,
    string ActorDisplayName,
    CacheAuditAction Action,
    string? Scope,
    int AffectedEntryCount);
