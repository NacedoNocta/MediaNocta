using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using CacheLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Website.Services;

internal sealed record CachedHttpResponse(int StatusCode, byte[] Body, string? ContentType);

public sealed class WebsiteCachingHandler : DelegatingHandler
{
    public static readonly HttpRequestOptionsKey<TimeSpan> CacheableTtl = new("WebsiteCaching.Ttl");

    private static readonly TimeSpan DefaultTtl = TimeSpan.FromDays(1);
    private static readonly TimeSpan CacheReadTimeout = TimeSpan.FromMilliseconds(50);
    private static readonly TimeSpan CacheWriteTimeout = TimeSpan.FromMilliseconds(200);

    private readonly IDistributedCache _cache;
    private readonly ICacheKeyBuilder _keyBuilder;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsMonitor<CachingOptions> _options;
    private readonly ILogger<WebsiteCachingHandler> _logger;

    public WebsiteCachingHandler(
        IDistributedCache cache,
        ICacheKeyBuilder keyBuilder,
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<CachingOptions> options,
        ILogger<WebsiteCachingHandler> logger)
    {
        _cache = cache;
        _keyBuilder = keyBuilder;
        _httpContextAccessor = httpContextAccessor;
        _options = options;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_options.CurrentValue.Enabled || !IsCacheableMethod(request) || !request.Options.TryGetValue(CacheableTtl, out var ttl))
        {
            PropagateBypassHeader(request);
            return await base.SendAsync(request, cancellationToken);
        }

        var bypassRequested = ShouldBypass();
        if (bypassRequested)
        {
            request.Headers.Remove(CacheHeaders.SkipCache);
            request.Headers.Add(CacheHeaders.SkipCache, "true");
            CacheTelemetry.RecordBypass(CacheNamespaces.Website, CacheProducers.WebsiteClient, request.RequestUri?.AbsolutePath);
        }

        var key = BuildKey(request);

        if (!bypassRequested)
        {
            var cachedBytes = await SafeGetAsync(key, cancellationToken);
            if (cachedBytes is not null)
            {
                var cached = JsonSerializer.Deserialize<CachedHttpResponse>(cachedBytes);
                if (cached is not null)
                {
                    CacheTelemetry.RecordHit(CacheNamespaces.Website, CacheProducers.WebsiteClient, request.RequestUri?.AbsolutePath);
                    return BuildResponse(cached, request);
                }
            }
            CacheTelemetry.RecordMiss(CacheNamespaces.Website, CacheProducers.WebsiteClient, request.RequestUri?.AbsolutePath);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            await TryStoreAsync(key, response, ttl, cancellationToken);
        }

        return response;
    }

    private static bool IsCacheableMethod(HttpRequestMessage request) =>
        request.Method == HttpMethod.Get || request.Method == HttpMethod.Head;

    private bool ShouldBypass()
    {
        var headers = _httpContextAccessor.HttpContext?.Request.Headers;
        return headers is not null && CacheHeaders.IsSkipCacheRequested(headers);
    }

    private void PropagateBypassHeader(HttpRequestMessage request)
    {
        if (ShouldBypass() && !request.Headers.Contains(CacheHeaders.SkipCache))
        {
            request.Headers.Add(CacheHeaders.SkipCache, "true");
        }
    }

    private string BuildKey(HttpRequestMessage request)
    {
        var route = request.RequestUri?.AbsolutePath ?? "/";
        var query = request.RequestUri?.Query ?? string.Empty;
        var variance = new Dictionary<string, string?>
        {
            ["culture"] = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
            ["query"] = query
        };
        return _keyBuilder.Build(CacheNamespaces.Website, CacheProducers.WebsiteClient, route, variance);
    }

    private async Task<byte[]?> SafeGetAsync(string key, CancellationToken ct)
    {
        // Hard timeout: if Redis is slow/unreachable, fall through to origin instead of hanging.
        // 50 ms is well under the resilience handler's attempt timeout, so a cache outage never
        // surfaces as a Polly TimeoutRejectedException to the page render path (NFR Reliability).
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(CacheReadTimeout);
        try
        {
            return await _cache.GetAsync(key, cts.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            _logger.LogDebug("Cache read timed out for {Key} - falling through to origin", key);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Distributed cache read failed for {Key}", key);
            return null;
        }
    }

    private async Task TryStoreAsync(string key, HttpResponseMessage response, TimeSpan ttl, CancellationToken ct)
    {
        try
        {
            var body = await response.Content.ReadAsByteArrayAsync(ct);
            var cached = new CachedHttpResponse((int)response.StatusCode, body, response.Content.Headers.ContentType?.ToString());
            var bytes = JsonSerializer.SerializeToUtf8Bytes(cached);

            // Hard timeout on store: a slow Redis must not extend response time beyond a small budget.
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(CacheWriteTimeout);
            try
            {
                await _cache.SetAsync(key, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, cts.Token);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                _logger.LogDebug("Cache write timed out for {Key} - response served, cache not updated", key);
            }

            response.Content = new ByteArrayContent(body);
            if (cached.ContentType is not null)
            {
                response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(cached.ContentType);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Distributed cache write failed for {Key}", key);
        }
    }

    private static HttpResponseMessage BuildResponse(CachedHttpResponse cached, HttpRequestMessage request)
    {
        var content = new ByteArrayContent(cached.Body);
        if (cached.ContentType is not null)
        {
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(cached.ContentType);
        }
        return new HttpResponseMessage((System.Net.HttpStatusCode)cached.StatusCode)
        {
            RequestMessage = request,
            Content = content
        };
    }

    public static TimeSpan DefaultTtlFor(string scope) => scope switch
    {
        _ => DefaultTtl
    };
}
