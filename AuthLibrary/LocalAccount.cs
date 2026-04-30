namespace AuthLibrary;

/// <summary>
/// Represents a local account in the system.
/// LocalAccount cannot be used for direct login - only accessible through Keycloak social login.
/// </summary>
public class LocalAccount
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public SocialAccount? SocialAccount { get; set; }
    public LocalSession? LocalSession { get; set; }
    public ICollection<LocalAccountRole> LocalAccountRoles { get; set; } = new List<LocalAccountRole>();
}
