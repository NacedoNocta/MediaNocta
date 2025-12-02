namespace AuthLibrary;

/// <summary>
/// Represents a role that can be assigned to local accounts.
/// Only two roles for prototype: "user" and "admin".
/// Synced from Keycloak realm roles.
/// </summary>
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<LocalAccountRole> LocalAccountRoles { get; set; } = new List<LocalAccountRole>();
}

/// <summary>
/// Standard role names used in the system
/// </summary>
public static class RoleNames
{
    public const string User = "user";
    public const string Admin = "admin";
    public const string BackofficeAdmin = "backoffice-admin";
}
