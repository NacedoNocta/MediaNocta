namespace AuthLibrary;

/// <summary>
/// Represents a local session for an authenticated user.
/// Tokens are encrypted with ASP.NET Data Protection API (keys from env vars).
/// Session duration is unlimited locally; actual duration controlled by Keycloak token expiration.
/// Only one active session per user - new session creation deletes previous session.
/// </summary>
public class LocalSession
{
    public Guid Id { get; set; }
    public Guid LocalAccountId { get; set; }

    // Encrypted tokens
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;

    // Token metadata for expiration checking
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public LocalAccount LocalAccount { get; set; } = null!;
}
