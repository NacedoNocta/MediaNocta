namespace AuthLibrary;

/// <summary>
/// Represents a social account linked to a local account.
/// One-to-one relationship with LocalAccount.
/// Keycloak handles multiple identity providers; Website only sees unified Keycloak identity.
/// </summary>
public class SocialAccount
{
    public Guid Id { get; set; }
    public Guid LocalAccountId { get; set; }
    public string Provider { get; set; } = "keycloak";
    public string ProviderUserId { get; set; } = string.Empty; // Keycloak user ID (sub claim)
    public string ProviderUsername { get; set; } = string.Empty;
    public string ProviderEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public LocalAccount LocalAccount { get; set; } = null!;
}
