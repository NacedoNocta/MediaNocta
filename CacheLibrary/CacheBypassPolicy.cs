using Microsoft.AspNetCore.Http;

namespace CacheLibrary;

public sealed class CacheBypassPolicy : ICacheBypassPolicy
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CacheBypassPolicy(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool ShouldBypass()
    {
        var headers = _httpContextAccessor.HttpContext?.Request.Headers;
        return headers is not null && CacheHeaders.IsSkipCacheRequested(headers);
    }
}
