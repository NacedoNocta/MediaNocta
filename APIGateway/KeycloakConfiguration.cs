using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace APIGateway;

public static class KeycloakConfiguration
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind Keycloak configuration from appsettings
        var keycloakOptions = new KeycloakOptions();
        configuration.GetSection(KeycloakOptions.SectionName).Bind(keycloakOptions);
        
        // Register the configuration for dependency injection
        services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.SectionName));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddKeycloakJwtBearer("keycloak", realm: keycloakOptions.Realm, options =>
            {
                options.Audience = keycloakOptions.Audience;
                options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;
                
                // Configure token validation parameters from settings
                options.TokenValidationParameters.ValidateIssuer = keycloakOptions.ValidateIssuer;
                options.TokenValidationParameters.ValidateAudience = keycloakOptions.ValidateAudience;
                options.TokenValidationParameters.ValidateLifetime = keycloakOptions.ValidateLifetime;
                options.TokenValidationParameters.ValidateIssuerSigningKey = keycloakOptions.ValidateIssuerSigningKey;
                options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(keycloakOptions.ClockSkewMinutes);
                options.TokenValidationParameters.RoleClaimType = keycloakOptions.RoleClaimType;
                options.TokenValidationParameters.NameClaimType = keycloakOptions.NameClaimType;
                
                // Map claims from Keycloak token
                options.MapInboundClaims = keycloakOptions.MapInboundClaims;
                
                // Configure events for comprehensive token handling
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Extract JWT token from Authorization header (Bearer token)
                        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authHeader["Bearer ".Length..].Trim();
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // Log authentication failures for debugging
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("JWT Authentication failed: {Exception}", context.Exception.Message);
                        
                        // Handle specific failure scenarios
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            logger.LogWarning("Token expired for request to {Path}", context.Request.Path);
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        else if (context.Exception is SecurityTokenSignatureKeyNotFoundException)
                        {
                            logger.LogError("Token signature validation failed - invalid signing key");
                        }
                        else if (context.Exception is SecurityTokenInvalidSignatureException)
                        {
                            logger.LogError("Token signature validation failed - signature mismatch");
                        }
                        
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogDebug("JWT token validated successfully for user: {User}", 
                            context.Principal?.FindFirst("preferred_username")?.Value ?? "Unknown");
                        
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            // Extract realm_access roles and add them as role claims
                            var realmAccessClaim = claimsIdentity.FindFirst("realm_access")?.Value;
                            if (!string.IsNullOrEmpty(realmAccessClaim))
                            {
                                try
                                {
                                    using var document = JsonDocument.Parse(realmAccessClaim);
                                    if (document.RootElement.TryGetProperty("roles", out var rolesElement))
                                    {
                                        foreach (var role in rolesElement.EnumerateArray())
                                        {
                                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.GetString() ?? ""));
                                        }
                                    }
                                }
                                catch (JsonException ex)
                                {
                                    logger.LogWarning("Failed to parse realm_access claim: {Exception}", ex.Message);
                                }
                            }
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("JWT Challenge issued for request to {Path}", context.Request.Path);
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    public static IServiceCollection AddKeycloakAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Policy for authenticated users only
            options.AddPolicy("AuthenticatedUser", policy =>
                policy.RequireAuthenticatedUser());

            // Policy for admin users (requires "website-admin" role in realm-access claim)
            options.AddPolicy("AdminUser", policy =>
                policy.RequireAuthenticatedUser()
                      .RequireAssertion(context => context.User.IsAdmin()));

            // Default policy for anonymous access (no authentication required)
            options.FallbackPolicy = null;
        });

        return services;
    }
}