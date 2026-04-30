using Backoffice.Authentication.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Backoffice.Authentication.Middleware;

/// <summary>
/// Middleware that checks access token expiration on each request and refreshes if needed.
/// Runs after authentication middleware and before authorization.
/// </summary>
public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenRefreshMiddleware> _logger;

    public TokenRefreshMiddleware(
        RequestDelegate next,
        ILogger<TokenRefreshMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        SessionManagementService sessionService,
        AccountManagementService accountService,
        IConfiguration configuration)
    {
        // Only process authenticated users
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            try
            {
                // Get session ID from claims
                var sessionIdClaim = context.User.FindFirst("session_id")?.Value;
                var accountIdClaim = context.User.FindFirst("account_id")?.Value;

                if (!string.IsNullOrEmpty(sessionIdClaim) && Guid.TryParse(sessionIdClaim, out var sessionId))
                {
                    var session = await sessionService.GetSessionByIdAsync(sessionId);

                    if (session == null)
                    {
                        _logger.LogWarning("Session not found in database, user will be logged out");
                        await context.SignOutAsync("Cookies");
                        context.Response.Redirect("/Account/Login");
                        return;
                    }

                    // Check if access token is expired
                    if (sessionService.IsAccessTokenExpired(session))
                    {
                        _logger.LogInformation("Access token expired for user {Username}, attempting refresh",
                            session.LocalAccount.Username);

                        // Check if refresh token is also expired
                        if (sessionService.IsRefreshTokenExpired(session))
                        {
                            _logger.LogWarning("Refresh token expired for user {Username}, session terminated",
                                session.LocalAccount.Username);

                            await sessionService.DeleteSessionAsync(session);
                            await context.SignOutAsync("Cookies");
                            context.Response.Redirect("/Account/Login");
                            return;
                        }

                        // Attempt token refresh
                        var refreshed = await RefreshTokensAsync(
                            session,
                            sessionService,
                            accountService,
                            configuration);

                        if (!refreshed)
                        {
                            _logger.LogWarning("Token refresh failed for user {Username}, session terminated",
                                session.LocalAccount.Username);

                            await sessionService.DeleteSessionAsync(session);
                            await context.SignOutAsync("Cookies");
                            context.Response.Redirect("/Account/Login");
                            return;
                        }

                        _logger.LogInformation("Successfully refreshed tokens for user {Username}",
                            session.LocalAccount.Username);
                    }
                    else
                    {
                        // Token still valid, just update last accessed time
                        await sessionService.UpdateLastAccessedAsync(session);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in token refresh middleware");
                // Continue processing - don't break the request
            }
        }

        await _next(context);
    }

    /// <summary>
    /// Refreshes tokens by calling Keycloak token endpoint
    /// </summary>
    private async Task<bool> RefreshTokensAsync(
        AuthLibrary.LocalSession session,
        SessionManagementService sessionService,
        AccountManagementService accountService,
        IConfiguration configuration)
    {
        try
        {
            // Get Keycloak configuration
            var keycloakUrl = configuration["services:keycloak:https:0"] ?? configuration["services:keycloak:http:0"];
            if (string.IsNullOrEmpty(keycloakUrl))
            {
                _logger.LogError("Keycloak URL not configured");
                return false;
            }

            var realm = configuration["Authentication:Keycloak:Realm"] ?? "medianocta";
            var clientId = configuration["Authentication:Keycloak:ClientId"] ?? "medianocta-web";
            var clientSecret = configuration["Authentication:Keycloak:ClientSecret"] ?? "";

            var tokenEndpoint = $"{keycloakUrl}/realms/{realm}/protocol/openid-connect/token";

            // Prepare refresh request
            var refreshToken = sessionService.GetRefreshToken(session);

            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", clientId }
            };

            if (!string.IsNullOrEmpty(clientSecret))
            {
                parameters.Add("client_secret", clientSecret);
            }

            request.Content = new FormUrlEncodedContent(parameters);

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Token refresh failed with status {Status}: {Error}",
                    response.StatusCode, error);
                return false;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = System.Text.Json.JsonDocument.Parse(responseContent);

            var newAccessToken = tokenResponse.RootElement.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("No access token in refresh response");

            var newRefreshToken = tokenResponse.RootElement.TryGetProperty("refresh_token", out var rt)
                ? rt.GetString() ?? refreshToken
                : refreshToken;

            // Update session with new tokens
            await sessionService.UpdateSessionTokensAsync(session, newAccessToken, newRefreshToken);

            // Parse new access token and sync roles
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            if (handler.CanReadToken(newAccessToken))
            {
                var jwt = handler.ReadJwtToken(newAccessToken);
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "jwt"));

                await accountService.SyncRolesAsync(session.LocalAccount, claimsPrincipal, newAccessToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during token refresh");
            return false;
        }
    }
}

/// <summary>
/// Extension method to add token refresh middleware to the pipeline
/// </summary>
public static class TokenRefreshMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenRefresh(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenRefreshMiddleware>();
    }
}
