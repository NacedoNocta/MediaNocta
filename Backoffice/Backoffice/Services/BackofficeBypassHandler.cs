using CacheLibrary;

namespace Backoffice.Services;

public sealed class BackofficeBypassHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains(CacheHeaders.SkipCache))
        {
            request.Headers.Add(CacheHeaders.SkipCache, "true");
        }
        return base.SendAsync(request, cancellationToken);
    }
}
