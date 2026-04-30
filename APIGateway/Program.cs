var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer("keycloak", realm: "medianocta", options =>
    {
        options.Audience = "medianocta-api";
        options.RequireHttpsMetadata = false; // For development only

        // Map Keycloak realm roles to .NET role claims
        // Keycloak stores roles in realm_access.roles JSON structure
        options.TokenValidationParameters.RoleClaimType = "role";

        // Extract roles from realm_access.roles and add them as role claims
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is System.Security.Claims.ClaimsIdentity identity)
                {
                    // Extract roles from realm_access.roles claim
                    var realmAccessClaim = context.Principal.FindFirst("realm_access");
                    if (realmAccessClaim != null)
                    {
                        try
                        {
                            var realmAccess = System.Text.Json.JsonDocument.Parse(realmAccessClaim.Value);
                            if (realmAccess.RootElement.TryGetProperty("roles", out var rolesElement))
                            {
                                foreach (var role in rolesElement.EnumerateArray())
                                {
                                    var roleName = role.GetString();
                                    if (!string.IsNullOrEmpty(roleName))
                                    {
                                        // Add role claim with the configured RoleClaimType
                                        identity.AddClaim(new System.Security.Claims.Claim("role", roleName));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var logger = context.HttpContext.RequestServices
                                .GetRequiredService<ILogger<Program>>();
                            logger.LogError(ex, "Failed to extract roles from realm_access claim");
                        }
                    }
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    // Policy for users with "user" role
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("user"));
    
    // Policy for users with "admin" role
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("website-admin"));
    
    // Policy for any authenticated user
    options.AddPolicy("AuthenticatedPolicy", policy =>
        policy.RequireAuthenticatedUser());
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.AddServiceDefaults();

var app = builder.Build();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapReverseProxy();

app.MapGet("/", () => "Hello World!");

app.Run();