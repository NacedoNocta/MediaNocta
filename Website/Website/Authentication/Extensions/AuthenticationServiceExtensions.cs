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
                    context.Response.Redirect("/");
                    context.HandleResponse();
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // Successful authentication - force redirect to home page
                    context.Properties.RedirectUri = "/";
                    return Task.CompletedTask;
                }
            };
        });

        // Add authorization
        services.AddAuthorization();

        // Register authentication services
        services.AddScoped<ICustomAuthenticationService, Website.Authentication.Services.AuthenticationService>();

        return services;
    }
}