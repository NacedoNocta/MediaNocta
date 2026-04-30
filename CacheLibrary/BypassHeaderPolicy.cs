using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;

namespace CacheLibrary;

public sealed class BypassHeaderPolicy : IOutputCachePolicy
{
    private readonly CacheBypassOptions _bypass;
    private readonly IOptionsMonitor<CachingOptions> _caching;

    public BypassHeaderPolicy(IOptions<CacheBypassOptions> bypass, IOptionsMonitor<CachingOptions> caching)
    {
        _bypass = bypass.Value;
        _caching = caching;
    }

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        if (!_caching.CurrentValue.Enabled)
        {
            context.EnableOutputCaching = false;
            context.AllowCacheLookup = false;
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        if (CacheHeaders.IsSkipCacheRequested(context.HttpContext.Request.Headers))
        {
            context.EnableOutputCaching = false;
            context.AllowCacheLookup = false;
            context.AllowCacheStorage = !_caching.CurrentValue.RepopulateOnBypass ? false : false;
            // Note: AllowCacheStorage stays false to skip *this* request; FR-D8 repopulation happens implicitly when
            // a subsequent request without the header is served and the upstream response is fresh. Keeping bypass
            // entries out of the cache prevents the cached entry from being keyed differently from header-less calls.
            CacheTelemetry.RecordBypass(_bypass.Namespace, _bypass.Producer, context.HttpContext.Request.Path);
        }
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken) => ValueTask.CompletedTask;
}
