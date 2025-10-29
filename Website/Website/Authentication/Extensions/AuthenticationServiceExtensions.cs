using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Website.Authentication.Configuration;
using Website.Authentication.Interfaces;
using Website.Authentication.Services;
using CustomAuthenticationOptions = Website.Authentication.Configuration.AuthenticationOptions;
using ICustomAuthenticationService = Website.Authentication.Interfaces.IAuthenticationService;

namespace Website.Authentication.Extensions;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind authentication configuration
        var authOptions = configuration.GetSection(CustomAuthenticationOptions.SectionName).Get<CustomAuthenticationOptions>() 
                         ?? new CustomAuthenticationOptions();

        // Add authentication schemes
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "OpenIdConnect";
        })
        .AddCookie("Cookies", options =>
        {
            options.LoginPath = authOptions.Cookies.LoginPath;
            options.LogoutPath = authOptions.Cookies.LogoutPath;
            options.AccessDeniedPath = authOptions.Cookies.AccessDeniedPath;
        })
        .AddKeycloakOpenIdConnect("keycloak", authOptions.Keycloak.Realm, "OpenIdConnect", options =>
        {
            options.ClientId = authOptions.Keycloak.ClientId;
            options.ClientSecret = authOptions.Keycloak.ClientSecret;
            options.ResponseType = "code";
            options.RequireHttpsMetadata = authOptions.Keycloak.RequireHttpsMetadata;
            options.SaveTokens = authOptions.Keycloak.SaveTokens;
            options.GetClaimsFromUserInfoEndpoint = authOptions.Keycloak.GetClaimsFromUserInfoEndpoint;
            options.SignInScheme = "Cookies";
            options.UsePkce = authOptions.Keycloak.UsePkce;
            
            // Configure scopes
            options.Scope.Clear();
            foreach (var scope in authOptions.Keycloak.Scopes ?? [])
            {
                options.Scope.Add(scope);
            }
            
            // Configure callback paths
            options.CallbackPath = authOptions.Keycloak.CallbackPath;
            options.SignedOutCallbackPath = authOptions.Keycloak.SignedOutCallbackPath;
            options.RemoteSignOutPath = authOptions.Keycloak.RemoteSignOutPath;
            
            // Configure authentication events
            options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("Authentication");
                    logger.LogError("Authentication failed: {Error}", context.Exception?.Message);
                    context.Response.Redirect("/");
                    context.HandleResponse();
                    return Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("Authentication");

                    try
                    {
                        logger.LogInformation("Token validated, processing account and session creation");

                        // Get required services
                        var accountService = context.HttpContext.RequestServices.GetRequiredService<AccountManagementService>();
                        var sessionService = context.HttpContext.RequestServices.GetRequiredService<SessionManagementService>();

                        // Extract tokens from context
                        var accessToken = context.TokenEndpointResponse?.AccessToken
                            ?? throw new InvalidOperationException("No access token in response");
                        var refreshToken = context.TokenEndpointResponse?.RefreshToken
                            ?? throw new InvalidOperationException("No refresh token in response");
                        var idToken = context.TokenEndpointResponse?.IdToken
                            ?? throw new InvalidOperationException("No ID token in response");

                        // Get or create local account and social account
                        var localAccount = await accountService.GetOrCreateAccountAsync(context.Principal!, idToken);

                        // Sync roles from token
                        await accountService.SyncRolesAsync(localAccount, context.Principal!, accessToken);

                        // Clean up expired sessions (basic cleanup for prototype)
                        await sessionService.CleanupExpiredSessionsAsync();

                        // Create new session (will delete existing session for single session enforcement)
                        var session = await sessionService.CreateSessionAsync(
                            localAccount,
                            accessToken,
                            refreshToken,
                            idToken);

                        // Store session ID in claims for later retrieval
                        var identity = context.Principal!.Identity as System.Security.Claims.ClaimsIdentity;
                        identity?.AddClaim(new System.Security.Claims.Claim("session_id", session.Id.ToString()));
                        identity?.AddClaim(new System.Security.Claims.Claim("account_id", localAccount.Id.ToString()));

                        logger.LogInformation("Successfully created session for user {Username}", localAccount.Username);

                        // Successful authentication - force redirect to home page
                        context.Properties.RedirectUri = "/";
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing authentication callback");
                        throw;
                    }
                }
            };
        });

        // Add authorization
        services.AddAuthorization();

        // Configure Data Protection for token encryption
        // For prototype: using default configuration (stores keys in-memory)
        // For V1: configure persistent key storage with environment variable paths
        services.AddDataProtection();

        // Register authentication services
        services.AddScoped<ICustomAuthenticationService, Website.Authentication.Services.AuthenticationService>();
        services.AddScoped<TokenEncryptionService>();
        services.AddScoped<AccountManagementService>();
        services.AddScoped<SessionManagementService>();

        return services;
    }
}