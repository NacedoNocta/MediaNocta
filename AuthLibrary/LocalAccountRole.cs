namespace AuthLibrary;

/// <summary>
/// Junction table for many-to-many relationship between LocalAccount and Role.
/// Roles synced from Keycloak realm_access.roles claim on every token receipt (login + refresh).
/// If role missing from token, it's removed from local account.
/// </summary>
public class LocalAccountRole
{
    public Guid LocalAccountId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public LocalAccount LocalAccount { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
