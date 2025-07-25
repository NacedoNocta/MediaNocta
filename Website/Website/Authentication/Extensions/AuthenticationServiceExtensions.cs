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
            
            // Custom event handlers to redirect to our error page
            options.Events.OnRedirectToLogin = context =>
            {
                // Instead of redirecting to login page, redirect to our unauthorized page
                context.Response.Redirect("/Unauthorized");
                return Task.CompletedTask;
            };
            
            options.Events.OnRedirectToAccessDenied = context =>
            {
                // Redirect to our unauthorized page for access denied scenarios
                context.Response.Redirect("/Unauthorized");
                return Task.CompletedTask;
            };
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
                    context.Response.Redirect("/");
                    context.HandleResponse();
                    return Task.CompletedTask;
                },
                OnTokenValidated = async context =>
                {
                    // Create session when tokens are validated
                    var sessionManager = context.HttpContext.RequestServices.GetRequiredService<ISessionManager>();
                    var accessToken = context.TokenEndpointResponse?.AccessToken ?? string.Empty;
                    var refreshToken = context.TokenEndpointResponse?.RefreshToken ?? string.Empty;
                    var idToken = context.TokenEndpointResponse?.IdToken ?? string.Empty;
                    
                    if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                    {
                        await sessionManager.CreateSessionAsync(context.Principal!, accessToken, refreshToken, idToken);
                    }
                    
                    // Use the return URL from the original authentication request if available
                    // Otherwise, redirect to home page
                    if (string.IsNullOrEmpty(context.Properties.RedirectUri))
                    {
                        context.Properties.RedirectUri = "/";
                    }
                }
            };
        });

        // Add authorization
        services.AddAuthorization();

        // Configure HTTP client for token refresh service
        services.AddHttpClient<ITokenRefreshService, TokenRefreshService>();
        
        // Register authentication services
        services.AddScoped<ICustomAuthenticationService, Website.Authentication.Services.AuthenticationService>();
        services.AddScoped<ISessionManager, SessionManager>();
        services.AddScoped<ITokenRefreshService, TokenRefreshService>();
        services.AddSingleton<TokenUpdateQueue>();
        services.AddSingleton<SessionStore>();

        // Configure authentication options
        services.Configure<CustomAuthenticationOptions>(
            configuration.GetSection(CustomAuthenticationOptions.SectionName));

        return services;
    }
}