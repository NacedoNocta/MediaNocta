using Website.Authentication.Services;
using System.Net.Http.Headers;

namespace Website.Authentication.Handlers;

/// <summary>
/// DelegatingHandler that automatically injects the access token from the session
/// into the Authorization header for authenticated API calls.
/// </summary>
public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AuthenticatedHttpClientHandler> _logger;

    public AuthenticatedHttpClientHandler(
        IHttpContextAccessor httpContextAccessor,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AuthenticatedHttpClientHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        // Only add token if user is authenticated
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            try
            {
                // Get session ID from claims
                var sessionIdClaim = httpContext.User.FindFirst("session_id")?.Value;

                if (!string.IsNullOrEmpty(sessionIdClaim) && Guid.TryParse(sessionIdClaim, out var sessionId))
                {
                    // Create a new scope to avoid DbContext concurrency issues
                    // Each concurrent HTTP request gets its own DbContext instance
                    using var scope = _serviceScopeFactory.CreateScope();
                    var sessionService = scope.ServiceProvider.GetRequiredService<SessionManagementService>();
                    var session = await sessionService.GetSessionByIdAsync(sessionId);

                    if (session != null)
                    {
                        // Get decrypted access token
                        var accessToken = sessionService.GetAccessToken(session);

                        // Add Bearer token to Authorization header
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        _logger.LogDebug("Added access token to request for {Method} {Uri}",
                            request.Method, request.RequestUri);
                    }
                    else
                    {
                        _logger.LogWarning("Session not found for authenticated user, request will be sent without token");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding access token to request");
                // Continue without token rather than failing the request
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
