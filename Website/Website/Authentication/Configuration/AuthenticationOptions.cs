namespace Website.Authentication.Configuration;

public class AuthenticationOptions
{
    public const string SectionName = "Authentication";
    
    public KeycloakOptions Keycloak { get; set; } = new();
    public CookieOptions Cookies { get; set; } = new();
}

public class KeycloakOptions
{
    public string Realm { get; set; } = "medianocta";
    public string ClientId { get; set; } = "medianocta-web";
    public string ClientSecret { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = false;
    public bool SaveTokens { get; set; } = true;
    public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;
    public bool UsePkce { get; set; } = true;
    public string[] Scopes { get; set; } = ["openid", "profile", "email", "roles"];
    public string CallbackPath { get; set; } = "/signin-oidc";
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";
    public string RemoteSignOutPath { get; set; } = "/signout-oidc";
}

public class CookieOptions
{
    public string LoginPath { get; set; } = "/Account/Login";
    public string LogoutPath { get; set; } = "/Account/Logout";
    public string AccessDeniedPath { get; set; } = "/Account/AccessDenied";
}