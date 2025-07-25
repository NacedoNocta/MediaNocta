using System.Security.Claims;
using System.Text.Json;

namespace Website.Authentication.Models;

public class UserSession
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public DateTime SessionCreated { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsKeycloakSessionExpired { get; set; } = false;
    public bool IsValid => !IsKeycloakSessionExpired && (DateTime.UtcNow < AccessTokenExpiration || DateTime.UtcNow < RefreshTokenExpiration);
    public bool IsAccessTokenExpired => DateTime.UtcNow >= AccessTokenExpiration;
    public bool IsRefreshTokenExpired => DateTime.UtcNow >= RefreshTokenExpiration;
    
    public static UserSession FromClaimsPrincipal(ClaimsPrincipal user, string accessToken, string refreshToken, string idToken)
    {
        var session = new UserSession
        {
            UserId = user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            UserName = user.Identity?.Name ?? string.Empty,
            Email = user.FindFirst("email")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            Roles = ExtractRolesFromToken(accessToken),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IdToken = idToken,
            SessionCreated = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow
        };
        
        // Parse token expiration from JWT if possible
        session.ParseTokenExpirations();
        
        return session;
    }
    
    public void UpdateFromClaimsPrincipal(ClaimsPrincipal user)
    {
        UserId = user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? UserId;
        UserName = user.Identity?.Name ?? UserName;
        Email = user.FindFirst("email")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value ?? Email;
        Roles = ExtractRolesFromToken(AccessToken);
        LastActivity = DateTime.UtcNow;
    }
    
    public void UpdateTokens(string accessToken, string refreshToken, string? idToken = null)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        if (!string.IsNullOrEmpty(idToken))
        {
            IdToken = idToken;
        }
        
        // Update roles from the new access token
        Roles = ExtractRolesFromToken(accessToken);
        
        LastActivity = DateTime.UtcNow;
        ParseTokenExpirations();
    }
    
    private void ParseTokenExpirations()
    {
        // Parse JWT token expiration (simplified - in production, use a proper JWT library)
        AccessTokenExpiration = ParseJwtExpiration(AccessToken) ?? DateTime.UtcNow.AddHours(1);
        RefreshTokenExpiration = ParseJwtExpiration(RefreshToken) ?? DateTime.UtcNow.AddDays(30);
    }
    
    private static DateTime? ParseJwtExpiration(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token)) return null;
            
            var parts = token.Split('.');
            if (parts.Length != 3) return null;
            
            var payload = parts[1];
            // Add padding if needed
            while (payload.Length % 4 != 0)
                payload += "=";
                
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            var jsonDoc = System.Text.Json.JsonDocument.Parse(json);
            
            if (jsonDoc.RootElement.TryGetProperty("exp", out var expElement))
            {
                var exp = expElement.GetInt64();
                return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        
        return null;
    }
    
    private static List<string> ExtractRolesFromToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token)) return new List<string>();
            
            var parts = token.Split('.');
            if (parts.Length != 3) return new List<string>();
            
            var payload = parts[1];
            // Add padding if needed
            while (payload.Length % 4 != 0)
                payload += "=";
                
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            var jsonDoc = JsonDocument.Parse(json);
            
            var roles = new List<string>();
            
            // Extract realm_access roles
            if (jsonDoc.RootElement.TryGetProperty("realm_access", out var realmAccess) &&
                realmAccess.TryGetProperty("roles", out var realmRoles) &&
                realmRoles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in realmRoles.EnumerateArray())
                {
                    var roleValue = role.GetString();
                    if (!string.IsNullOrEmpty(roleValue))
                    {
                        roles.Add(roleValue);
                    }
                }
            }
            
            return roles;
        }
        catch
        {
            // Return empty list on parsing errors
            return new List<string>();
        }
    }
}