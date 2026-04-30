using StackExchange.Redis;

namespace CacheLibrary;

public static class CacheRedisDefaults
{
    // Fail-fast SE.Redis configuration applied to every Aspire Redis client integration.
    // AbortOnConnectFail=false is the load-bearing knob: it lets the multiplexer lazily
    // reconnect when Redis comes back, instead of throwing at boot when Redis is briefly
    // unreachable. Sub-second timeouts ensure each cache call fails fast (well under the
    // resilience handler's 10s AttemptTimeout) so a Redis outage degrades to direct
    // origin reads rather than a Polly TimeoutRejectedException at the page render.
    public static void ApplyFailFast(ConfigurationOptions options)
    {
        options.AbortOnConnectFail = false;
        options.ConnectTimeout = 500;
        options.SyncTimeout = 500;
        options.AsyncTimeout = 500;
        options.ConnectRetry = 1;
    }
}
