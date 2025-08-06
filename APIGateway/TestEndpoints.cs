using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace APIGateway;

public static class TestEndpoints
{
    public static void MapTestEndpoints(this WebApplication app)
    {
        // Public endpoint - accessible to everyone (non-authenticated users)
        app.MapGet("/", () => "Hello World! - Public access");

        // Anonymous endpoint - explicitly allows non-authenticated access
        app.MapGet("/api/test/public", () => "Public endpoint - no authentication required")
            .AllowAnonymous();

        // Authenticated user endpoint - requires valid JWT token
        app.MapGet("/api/test/authenticated", () => "Authenticated user endpoint accessed successfully!")
            .RequireAuthorization("AuthenticatedUser");

        // Admin endpoint - requires "website-admin" role in realm-access claim
        app.MapGet("/api/test/admin", () => "Admin endpoint accessed successfully!")
            .RequireAuthorization("AdminUser");

        // User info endpoint - shows user claims for debugging
        app.MapGet("/api/test/userinfo", (ClaimsPrincipal user) =>
        {
            if (!user.IsAuthenticated())
                return Results.Json(new { message = "Not authenticated", isAuthenticated = false });

            var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Results.Json(new
            {
                message = "User information",
                isAuthenticated = true,
                isAdmin = user.IsAdmin(),
                userId = user.GetUserId(),
                username = user.GetUsername(),
                email = user.GetEmail(),
                roles = user.FindFirst("realm_access")?.Value,
                allClaims = claims
            });
        }).RequireAuthorization("AuthenticatedUser");

        // Authentication test endpoint - detailed token validation info
        app.MapGet("/api/test/auth-details", (HttpContext context, ClaimsPrincipal user) =>
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            var hasAuthHeader = !string.IsNullOrEmpty(authHeader);
            var hasBearerToken = hasAuthHeader && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
            
            return Results.Json(new
            {
                message = "Authentication details",
                hasAuthorizationHeader = hasAuthHeader,
                hasBearerToken = hasBearerToken,
                authHeaderValue = hasAuthHeader ? authHeader[..Math.Min(20, authHeader.Length)] + "..." : null,
                isAuthenticated = user.IsAuthenticated(),
                isAdmin = user.IsAdmin(),
                tokenClaims = user.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                authenticationScheme = user.Identity?.AuthenticationType,
                username = user.GetUsername(),
                userId = user.GetUserId()
            });
        }).AllowAnonymous();

        // Configuration endpoint - shows current Keycloak configuration
        app.MapGet("/api/test/config", (IOptions<KeycloakOptions> keycloakOptions) =>
        {
            var config = keycloakOptions.Value;
            return Results.Json(new
            {
                message = "Current Keycloak configuration",
                configuration = new
                {
                    realm = config.Realm,
                    audience = config.Audience,
                    requireHttpsMetadata = config.RequireHttpsMetadata,
                    validateIssuer = config.ValidateIssuer,
                    validateAudience = config.ValidateAudience,
                    validateLifetime = config.ValidateLifetime,
                    validateIssuerSigningKey = config.ValidateIssuerSigningKey,
                    clockSkewMinutes = config.ClockSkewMinutes,
                    roleClaimType = config.RoleClaimType,
                    nameClaimType = config.NameClaimType,
                    mapInboundClaims = config.MapInboundClaims
                },
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        }).AllowAnonymous();
    }
}