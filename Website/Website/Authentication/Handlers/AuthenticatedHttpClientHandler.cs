using Website.Authentication.Services;
using System.Net.Http.Headers;

namespace Website.Authentication.Handlers;

/// <summary>
/// DelegatingHandler that automatically injects the access token from the session
/// into the Authorization header for authenticated API calls.
/// Header format: "Authorization: Bearer {token}"
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
        // Skip if the request already has an Authorization header
        if (request.Headers.Authorization != null)
        {
            _logger.LogDebug("Request already has Authorization header, skipping token injection");
            return await base.SendAsync(request, cancellationToken);
        }

        var httpContext = _httpContextAccessor.HttpContext;

        // Only add token if user is authenticated
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            try
            {
                // Get session ID from claims
                var sessionIdClaim = httpContext.User.FindFirst("session_id")?.Value;

                if (string.IsNullOrEmpty(sessionIdClaim))
                {
                    _logger.LogWarning("User is authenticated but session_id claim is missing");
                    return await base.SendAsync(request, cancellationToken);
                }

                if (!Guid.TryParse(sessionIdClaim, out var sessionId))
                {
                    _logger.LogWarning("Failed to parse session_id claim: {SessionIdClaim}", sessionIdClaim);
                    return await base.SendAsync(request, cancellationToken);
                }

                // Create a new scope to avoid DbContext concurrency issues
                // Each concurrent HTTP request gets its own DbContext instance
                using var scope = _serviceScopeFactory.CreateScope();
                var sessionService = scope.ServiceProvider.GetRequiredService<SessionManagementService>();
                var session = await sessionService.GetSessionByIdAsync(sessionId);

                if (session == null)
                {
                    _logger.LogWarning("Session {SessionId} not found for authenticated user, request will be sent without token", sessionId);
                    return await base.SendAsync(request, cancellationToken);
                }

                // Check if token is expired
                if (sessionService.IsAccessTokenExpired(session))
                {
                    _logger.LogWarning("Access token expired for session {SessionId}, token refresh should occur", sessionId);
                    // Note: Token refresh is handled by TokenRefreshMiddleware
                    // If we reach here, the middleware hasn't refreshed yet or failed
                    // Continue with expired token - the API will return 401 and trigger refresh
                }

                // Get decrypted access token
                var accessToken = sessionService.GetAccessToken(session);

                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogError("Access token is null or empty for session {SessionId}", sessionId);
                    return await base.SendAsync(request, cancellationToken);
                }

                // Add Bearer token to Authorization header
                // Standard format: "Authorization: Bearer {token}"
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                _logger.LogDebug("Added Bearer token to Authorization header for {Method} {Uri}",
                    request.Method, request.RequestUri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding access token to request for {Method} {Uri}",
                    request.Method, request.RequestUri);
                // Continue without token rather than failing the request
                // The API will return 401 if authentication is required
            }
        }
        else
        {
            _logger.LogDebug("User not authenticated, sending request without Authorization header to {Method} {Uri}",
                request.Method, request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
