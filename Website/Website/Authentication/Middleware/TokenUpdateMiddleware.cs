using Microsoft.AspNetCore.Authentication;
using Website.Authentication.Interfaces;
using Website.Authentication.Services;

namespace Website.Authentication.Middleware;

public class TokenUpdateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenUpdateMiddleware> _logger;

    public TokenUpdateMiddleware(RequestDelegate next, ILogger<TokenUpdateMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, TokenUpdateQueue tokenUpdateQueue, ISessionManager sessionManager)
    {
        // Skip processing for non-authenticated users or static assets
        if (context.User.Identity?.IsAuthenticated != true || IsStaticAsset(context.Request.Path))
        {
            await _next(context);
            return;
        }

        try
        {
            // Check if current access token is expired and refresh if needed
            await CheckAndRefreshExpiredTokenAsync(context, sessionManager);

            // Process any pending token updates at the start of the request
            if (tokenUpdateQueue.HasPendingUpdates)
            {
                await ProcessPendingTokenUpdates(context, tokenUpdateQueue);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in token update middleware");
        }

        await _next(context);
    }

    private async Task ProcessPendingTokenUpdates(HttpContext context, TokenUpdateQueue tokenUpdateQueue)
    {
        try
        {
            var currentUserId = context.User.FindFirst("sub")?.Value ?? 
                              context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(currentUserId))
            {
                return;
            }

            var update = tokenUpdateQueue.DequeueTokenUpdate();
            if (update == null || update.UserId != currentUserId)
            {
                return;
            }

            _logger.LogDebug("Processing token update for user: {UserId}", currentUserId);

            // Get current authentication result
            var authResult = await context.AuthenticateAsync();
            if (!authResult.Succeeded)
            {
                _logger.LogWarning("Cannot update tokens - authentication result failed for user: {UserId}", currentUserId);
                return;
            }

            // Update the tokens in the authentication properties
            var tokens = authResult.Properties.GetTokens().ToList();
            tokens.RemoveAll(t => t.Name == "access_token" || t.Name == "refresh_token" || t.Name == "id_token");
            tokens.Add(new AuthenticationToken { Name = "access_token", Value = update.AccessToken });
            tokens.Add(new AuthenticationToken { Name = "refresh_token", Value = update.RefreshToken });
            
            if (!string.IsNullOrEmpty(update.IdToken))
            {
                tokens.Add(new AuthenticationToken { Name = "id_token", Value = update.IdToken });
            }

            authResult.Properties.StoreTokens(tokens);
            
            // Re-sign in with updated tokens
            await context.SignInAsync(authResult.Principal, authResult.Properties);
            
            _logger.LogInformation("Successfully updated authentication context with new tokens for user: {UserId}", currentUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing token update");
        }
    }

    private async Task CheckAndRefreshExpiredTokenAsync(HttpContext context, ISessionManager sessionManager)
    {
        try
        {
            // Get current session to check token expiration
            var session = await sessionManager.GetCurrentSessionAsync();
            
            if (session == null)
            {
                _logger.LogDebug("No session found, user may need to re-authenticate");
                return;
            }

            // Skip if Keycloak session is already expired
            if (session.IsKeycloakSessionExpired)
            {
                _logger.LogDebug("Keycloak session expired for user: {UserId}, terminating session", session.UserId);
                await TerminateSession(context, sessionManager);
                return;
            }

            // Check if access token is expired
            if (session.IsAccessTokenExpired)
            {
                _logger.LogDebug("Access token expired for user: {UserId}, attempting refresh", session.UserId);
                
                var refreshResult = await sessionManager.RefreshTokensAsync();
                
                if (!refreshResult.Success)
                {
                    _logger.LogWarning("Failed to refresh expired token for user: {UserId}, Error: {Error}", 
                        session.UserId, refreshResult.ErrorMessage);
                    
                    // If refresh failed, terminate the session
                    await TerminateSession(context, sessionManager);
                    return;
                }
                
                _logger.LogInformation("Successfully refreshed expired token for user: {UserId}", session.UserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking and refreshing expired token");
        }
    }

    private async Task TerminateSession(HttpContext context, ISessionManager sessionManager)
    {
        try
        {
            _logger.LogInformation("Terminating session due to token refresh failure or expiry");
            
            // Clear the session
            await sessionManager.ClearSessionAsync();
            
            // Sign out from local cookie
            await context.SignOutAsync("Cookies");
            
            // Instead of forcing redirect, let the request continue
            // The authorization will be handled by [Authorize] attributes and proper error pages
            _logger.LogDebug("Session terminated, allowing request to continue for proper error handling");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating session");
        }
    }

    private static bool IsStaticAsset(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant();
        if (string.IsNullOrEmpty(pathValue))
            return false;

        // Skip common static asset paths
        return pathValue.StartsWith("/_framework/") ||
               pathValue.StartsWith("/_content/") ||
               pathValue.StartsWith("/css/") ||
               pathValue.StartsWith("/js/") ||
               pathValue.StartsWith("/images/") ||
               pathValue.StartsWith("/favicon.ico") ||
               pathValue.EndsWith(".css") ||
               pathValue.EndsWith(".js") ||
               pathValue.EndsWith(".png") ||
               pathValue.EndsWith(".jpg") ||
               pathValue.EndsWith(".jpeg") ||
               pathValue.EndsWith(".gif") ||
               pathValue.EndsWith(".svg") ||
               pathValue.EndsWith(".ico") ||
               pathValue.EndsWith(".woff") ||
               pathValue.EndsWith(".woff2") ||
               pathValue.EndsWith(".ttf") ||
               pathValue.EndsWith(".eot");
    }
}