using AuthLibrary;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using DatabaseManager.DbContexts;

namespace Backoffice.Authentication.Services;

/// <summary>
/// Service for managing local accounts, social accounts, and roles.
/// Handles account creation, linking, and role synchronization from Keycloak tokens.
/// </summary>
public class AccountManagementService
{
    private readonly AuthDbContext _dbContext;
    private readonly ILogger<AccountManagementService> _logger;

    public AccountManagementService(
        AuthDbContext dbContext,
        ILogger<AccountManagementService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Gets or creates a local account from Keycloak token claims.
    /// Auto-creates LocalAccount and SocialAccount on first login.
    /// </summary>
    public async Task<LocalAccount> GetOrCreateAccountAsync(ClaimsPrincipal user, string idToken)
    {
        // Extract claims from token
        var sub = user.FindFirst("sub")?.Value
                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? throw new InvalidOperationException("No subject claim found in token");

        var username = user.FindFirst("preferred_username")?.Value
                       ?? user.FindFirst(ClaimTypes.Name)?.Value
                       ?? user.Identity?.Name
                       ?? throw new InvalidOperationException("No username claim found in token");

        var email = user.FindFirst("email")?.Value
                    ?? user.FindFirst(ClaimTypes.Email)?.Value
                    ?? throw new InvalidOperationException("No email claim found in token");

        _logger.LogInformation("Processing account for Keycloak user: {Sub}, Username: {Username}", sub, username);

        // Check if social account exists
        var socialAccount = await _dbContext.SocialAccounts
            .Include(sa => sa.LocalAccount)
            .FirstOrDefaultAsync(sa => sa.Provider == "keycloak" && sa.ProviderUserId == sub);

        if (socialAccount != null)
        {
            // Update social account info
            socialAccount.ProviderUsername = username;
            socialAccount.ProviderEmail = email;
            socialAccount.UpdatedAt = DateTime.UtcNow;

            // Update local account info
            socialAccount.LocalAccount.Username = username;
            socialAccount.LocalAccount.Email = email;
            socialAccount.LocalAccount.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated existing account for user: {Username}", username);
            return socialAccount.LocalAccount;
        }

        // Create new local account and social account
        var localAccount = new LocalAccount
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var newSocialAccount = new SocialAccount
        {
            Id = Guid.NewGuid(),
            LocalAccountId = localAccount.Id,
            Provider = "keycloak",
            ProviderUserId = sub,
            ProviderUsername = username,
            ProviderEmail = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.LocalAccounts.Add(localAccount);
        _dbContext.SocialAccounts.Add(newSocialAccount);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Created new local account for user: {Username}", username);
        return localAccount;
    }

    /// <summary>
    /// Syncs roles from Keycloak token to local account.
    /// Adds missing roles and removes roles not present in token.
    /// </summary>
    public async Task SyncRolesAsync(LocalAccount account, ClaimsPrincipal user, string accessToken)
    {
        // Extract roles from token
        var tokenRoles = ExtractRolesFromToken(user, accessToken);

        _logger.LogInformation("Syncing roles for user {Username}. Token roles: {Roles}",
            account.Username, string.Join(", ", tokenRoles));

        // Get existing roles for this account
        var existingAccountRoles = await _dbContext.LocalAccountRoles
            .Include(lar => lar.Role)
            .Where(lar => lar.LocalAccountId == account.Id)
            .ToListAsync();

        var existingRoleNames = existingAccountRoles.Select(ar => ar.Role.Name).ToHashSet();

        // Remove roles not in token
        var rolesToRemove = existingAccountRoles
            .Where(ar => !tokenRoles.Contains(ar.Role.Name))
            .ToList();

        if (rolesToRemove.Any())
        {
            _dbContext.LocalAccountRoles.RemoveRange(rolesToRemove);
            _logger.LogInformation("Removing roles for user {Username}: {Roles}",
                account.Username, string.Join(", ", rolesToRemove.Select(r => r.Role.Name)));
        }

        // Add roles from token that don't exist locally
        var rolesToAdd = tokenRoles.Except(existingRoleNames).ToList();

        foreach (var roleName in rolesToAdd)
        {
            // Ensure role exists in database
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    Description = $"Auto-created role: {roleName}",
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.Roles.Add(role);
                await _dbContext.SaveChangesAsync();
            }

            // Add role to account
            var accountRole = new LocalAccountRole
            {
                LocalAccountId = account.Id,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            };
            _dbContext.LocalAccountRoles.Add(accountRole);

            _logger.LogInformation("Adding role {Role} to user {Username}", roleName, account.Username);
        }

        if (rolesToAdd.Any() || rolesToRemove.Any())
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extracts roles from Keycloak token claims.
    /// Checks both realm_access.roles (from JWT) and role claims.
    /// </summary>
    private List<string> ExtractRolesFromToken(ClaimsPrincipal user, string accessToken)
    {
        var roles = new HashSet<string>();

        // Try to get roles from standard role claims
        var roleClaims = user.FindAll(ClaimTypes.Role).Select(c => c.Value);
        foreach (var role in roleClaims)
        {
            roles.Add(role);
        }

        // Try to parse JWT and extract realm_access.roles
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(accessToken))
            {
                var jwt = handler.ReadJwtToken(accessToken);

                // Look for realm_access claim
                var realmAccess = jwt.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
                if (!string.IsNullOrEmpty(realmAccess))
                {
                    // Parse JSON to extract roles array
                    var doc = System.Text.Json.JsonDocument.Parse(realmAccess);
                    if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                    {
                        foreach (var role in rolesElement.EnumerateArray())
                        {
                            var roleName = role.GetString();
                            if (!string.IsNullOrEmpty(roleName))
                            {
                                roles.Add(roleName);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse JWT token for role extraction");
        }

        // Filter to only include user, admin, and backoffice-admin roles
        return roles.Where(r => r == RoleNames.User || r == RoleNames.Admin || r == RoleNames.BackofficeAdmin).ToList();
    }

    /// <summary>
    /// Gets a local account by ID
    /// </summary>
    public async Task<LocalAccount?> GetAccountByIdAsync(Guid accountId)
    {
        return await _dbContext.LocalAccounts
            .Include(a => a.LocalAccountRoles)
                .ThenInclude(ar => ar.Role)
            .FirstOrDefaultAsync(a => a.Id == accountId);
    }
}
