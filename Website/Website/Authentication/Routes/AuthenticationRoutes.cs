using Microsoft.AspNetCore.Authentication;
using Website.Authentication.Interfaces;

namespace Website.Authentication.Routes;

public static class AuthenticationRoutes
{
    public const string LoginPath = "/Account/Login";
    public const string LogoutPath = "/Account/Logout";
    public const string AuthStatusPath = "/auth/status";
    public const string DebugTokenPath = "/debug/token";
    
    private static string GetSafeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || !returnUrl.StartsWith("/"))
        {
            return "/";
        }
        
        // Additional security: ensure URL doesn't contain protocol schemes
        if (returnUrl.Contains("://"))
        {
            return "/";
        }
        
        return returnUrl;
    }
    
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        // Login endpoint
        app.MapGet(LoginPath, (HttpContext context) =>
        {
            var returnUrl = GetSafeReturnUrl(context.Request.Query["ReturnUrl"].ToString());
            
            // If already authenticated, redirect to home immediately
            if (context.User.Identity?.IsAuthenticated == true)
            {
                return Results.Redirect(returnUrl);
            }

            // Store the return URL for after authentication
            var authProperties = new AuthenticationProperties 
            { 
                RedirectUri = returnUrl,
                IsPersistent = true
            };

            return Results.Challenge(authProperties, new[] { "OpenIdConnect" });
        })
        .WithName("Login")
        .WithDisplayName("User Login");

        // Logout endpoint
        app.MapGet(LogoutPath, async (HttpContext context, ISessionManager sessionManager) =>
        {
            var returnUrl = GetSafeReturnUrl(context.Request.Query["ReturnUrl"].ToString());
            
            // Clear local session first
            await sessionManager.ClearSessionAsync();
            
            // Sign out from local cookie
            await context.SignOutAsync("Cookies");
            
            // Sign out from Keycloak
            await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        })
        .WithName("Logout")
        .WithDisplayName("User Logout");

        // Debug endpoint to check authentication state and tokens
        app.MapGet(DebugTokenPath, async (HttpContext context, ISessionManager sessionManager) =>
        {
            var session = await sessionManager.GetCurrentSessionAsync();
            
            if (session != null)
            {
                return Results.Json(new
                {
                    IsAuthenticated = true,
                    UserName = session.UserName,
                    Email = session.Email,
                    UserId = session.UserId,
                    Roles = session.Roles,
                    SessionCreated = session.SessionCreated,
                    LastActivity = session.LastActivity,
                    AccessTokenExpiration = session.AccessTokenExpiration,
                    RefreshTokenExpiration = session.RefreshTokenExpiration,
                    IsAccessTokenExpired = session.IsAccessTokenExpired,
                    IsRefreshTokenExpired = session.IsRefreshTokenExpired,
                    AccessTokenPreview = !string.IsNullOrEmpty(session.AccessToken) 
                        ? session.AccessToken.Substring(0, Math.Min(100, session.AccessToken.Length)) + "..." 
                        : null
                });
            }
            
            return Results.Json(new { IsAuthenticated = false });
        })
        .RequireAuthorization()
        .WithName("DebugToken")
        .WithDisplayName("Debug Authentication Token");

        // Authentication status endpoint
        app.MapGet(AuthStatusPath, (HttpContext context) =>
        {
            return Results.Json(new
            {
                IsAuthenticated = context.User.Identity?.IsAuthenticated == true,
                UserName = context.User.Identity?.Name,
                AuthenticationType = context.User.Identity?.AuthenticationType
            });
        })
        .WithName("AuthStatus")
        .WithDisplayName("Authentication Status");

        // Post-authentication callback handler
        app.MapGet("/auth/callback", (HttpContext context) =>
        {
            // After successful authentication, redirect to home
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var returnUrl = GetSafeReturnUrl(context.Request.Query["ReturnUrl"].ToString());
                return Results.Redirect(returnUrl);
            }
            return Results.Redirect(LoginPath);
        })
        .WithName("AuthCallback")
        .WithDisplayName("Authentication Callback");
    }
}