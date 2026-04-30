using System.Text;
using CacheAdminApi.Services;
using CacheLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CacheAdminApi.Controllers;

[ApiController]
[Route("cache-admin")]
[Authorize(Policy = "AdminPolicy")]
public class CacheAdminController : ControllerBase
{
    private readonly IRedisInspector _inspector;
    private readonly ILogger<CacheAdminController> _logger;

    public CacheAdminController(IRedisInspector inspector, ILogger<CacheAdminController> logger)
    {
        _inspector = inspector;
        _logger = logger;
    }

    [HttpGet("groups")]
    public async Task<ActionResult<CacheGroupsResponse>> GetGroups(CancellationToken ct)
    {
        var groups = await _inspector.GetGroupsAsync(ct);
        return Ok(new CacheGroupsResponse(groups));
    }

    [HttpGet("entries")]
    public async Task<ActionResult<CacheEntriesResponse>> GetEntries(
        [FromQuery] string? @namespace,
        [FromQuery] string? producer,
        [FromQuery] string? routePrefix,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 500) pageSize = 500;

        var totalCount = await _inspector.CountEntriesAsync(@namespace, producer, routePrefix, ct);
        var entries = await _inspector.GetEntriesAsync(@namespace, producer, routePrefix, page, pageSize, ct);
        return Ok(new CacheEntriesResponse(entries, totalCount, page, pageSize));
    }

    [HttpDelete("entries/{keyBase64}")]
    public async Task<IActionResult> DeleteEntry(string keyBase64, CancellationToken ct)
    {
        if (!TryDecodeKey(keyBase64, out var key))
        {
            return BadRequest("Invalid base64 key.");
        }

        var deleted = await _inspector.EvictAsync(key, ct);
        WriteAudit(CacheAuditAction.EvictEntry, key, deleted ? 1 : 0);
        return deleted ? NoContent() : NotFound();
    }

    [HttpDelete("groups")]
    public async Task<ActionResult<CacheEvictionResponse>> DeleteGroup(
        [FromQuery] string @namespace,
        [FromQuery] string producer,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(@namespace) || string.IsNullOrWhiteSpace(producer))
        {
            return BadRequest("Both 'namespace' and 'producer' are required.");
        }

        var prefix = $"{CacheKeys.SolutionPrefix}:{@namespace}:{producer}:";
        var count = await _inspector.EvictByPrefixAsync(prefix, ct);
        WriteAudit(CacheAuditAction.EvictGroup, $"{@namespace}/{producer}", count);
        return Ok(new CacheEvictionResponse(count));
    }

    [HttpDelete("all")]
    public async Task<ActionResult<CacheEvictionResponse>> DeleteAll([FromQuery] string? confirm, CancellationToken ct)
    {
        if (confirm != "YES_FLUSH")
        {
            return BadRequest("Pass ?confirm=YES_FLUSH to acknowledge this destructive action.");
        }

        var count = await _inspector.EvictByPrefixAsync($"{CacheKeys.SolutionPrefix}:", ct);
        WriteAudit(CacheAuditAction.FlushAll, "*", count);
        return Ok(new CacheEvictionResponse(count));
    }

    private static bool TryDecodeKey(string keyBase64, out string key)
    {
        try
        {
            var padded = keyBase64.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            key = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            return true;
        }
        catch
        {
            key = string.Empty;
            return false;
        }
    }

    private void WriteAudit(CacheAuditAction action, string scope, int affected)
    {
        var actorId = User.FindFirst("sub")?.Value ?? "unknown";
        var actorName = User.Identity?.Name ?? "unknown";
        var evt = new CacheAuditEvent(DateTimeOffset.UtcNow, actorId, actorName, action, scope, affected);
        using var activity = CacheTelemetry.ActivitySource.StartActivity($"cache.admin.{action}");
        activity?.SetTag("cache.admin.actor", actorId);
        activity?.SetTag("cache.admin.scope", scope);
        activity?.SetTag("cache.admin.affected_count", affected);
        _logger.LogInformation("Cache admin action {Action} by {Actor} on {Scope} affected {Count} entries", evt.Action, evt.ActorDisplayName, evt.Scope, evt.AffectedEntryCount);
    }
}
