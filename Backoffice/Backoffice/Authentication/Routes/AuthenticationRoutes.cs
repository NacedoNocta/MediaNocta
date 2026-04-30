using Microsoft.AspNetCore.Authentication;

namespace Backoffice.Authentication.Routes;

public static class AuthenticationRoutes
{
    public const string LoginPath = "/Account/Login";
    public const string LogoutPath = "/Account/Logout";
    public const string AuthStatusPath = "/auth/status";
    
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
        .AllowAnonymous()  // Must be accessible to unauthenticated users
        .WithName("Login")
        .WithDisplayName("User Login");

        // Logout endpoint - requires authentication but NOT the backoffice-admin role
        app.MapGet(LogoutPath, async (HttpContext context) =>
        {
            // Delete local session if exists
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var accountIdClaim = context.User.FindFirst("account_id")?.Value;
                if (!string.IsNullOrEmpty(accountIdClaim) && Guid.TryParse(accountIdClaim, out var accountId))
                {
                    var sessionService = context.RequestServices.GetRequiredService<Backoffice.Authentication.Services.SessionManagementService>();
                    await sessionService.DeleteSessionByAccountIdAsync(accountId);
                }
            }

            await context.SignOutAsync("Cookies");
            await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        })
        .RequireAuthorization("AuthenticatedOnly")  // Only requires authentication, not role
        .WithName("Logout")
        .WithDisplayName("User Logout");

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
        .AllowAnonymous()
        .WithName("AuthCallback")
        .WithDisplayName("Authentication Callback");
    }
}