using Microsoft.AspNetCore.Authentication;

namespace Website.Authentication.Routes;

public static class AuthenticationRoutes
{
    public const string LoginPath = "/Account/Login";
    public const string LogoutPath = "/Account/Logout";
    public const string AuthStatusPath = "/auth/status";
    public const string DebugTokenPath = "/debug/token";
    
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        // Login endpoint
        app.MapGet(LoginPath, (HttpContext context) =>
        {
            var returnUrl = context.Request.Query["ReturnUrl"].ToString() ?? "/";
            
            // If already authenticated, redirect to home immediately
            if (context.User.Identity?.IsAuthenticated == true)
            {
                return Results.Redirect(returnUrl);
            }

            // Store the return URL for after authentication
            var authProperties = new AuthenticationProperties 
            { 
                RedirectUri = "/",
                IsPersistent = true
            };

            return Results.Challenge(authProperties, new[] { "OpenIdConnect" });
        })
        .WithName("Login")
        .WithDisplayName("User Login");

        // Logout endpoint
        app.MapGet(LogoutPath, async (HttpContext context) =>
        {
            await context.SignOutAsync("Cookies");
            await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        })
        .WithName("Logout")
        .WithDisplayName("User Logout");

        // Debug endpoint to check authentication state and tokens
        app.MapGet(DebugTokenPath, async (HttpContext context) =>
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var accessToken = await context.GetTokenAsync("access_token");
                var idToken = await context.GetTokenAsync("id_token");
                
                return Results.Json(new
                {
                    IsAuthenticated = true,
                    UserName = context.User.Identity.Name,
                    Claims = context.User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                    HasAccessToken = !string.IsNullOrEmpty(accessToken),
                    HasIdToken = !string.IsNullOrEmpty(idToken),
                    AccessTokenPreview = !string.IsNullOrEmpty(accessToken) 
                        ? accessToken.Substring(0, Math.Min(100, accessToken.Length)) + "..." 
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
                return Results.Redirect("/");
            }
            return Results.Redirect(LoginPath);
        })
        .WithName("AuthCallback")
        .WithDisplayName("Authentication Callback");
    }
}