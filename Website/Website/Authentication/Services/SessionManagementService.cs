using AuthLibrary;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using DatabaseManager.DbContexts;

namespace Website.Authentication.Services;

/// <summary>
/// Service for managing local sessions.
/// Handles session creation, token storage/retrieval, refresh, and cleanup.
/// Enforces single session per user policy.
/// </summary>
public class SessionManagementService
{
    private readonly AuthDbContext _dbContext;
    private readonly TokenEncryptionService _encryptionService;
    private readonly ILogger<SessionManagementService> _logger;

    public SessionManagementService(
        AuthDbContext dbContext,
        TokenEncryptionService encryptionService,
        ILogger<SessionManagementService> logger)
    {
        _dbContext = dbContext;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new session for the given account.
    /// Deletes any existing session for the account (single session enforcement).
    /// </summary>
    public async Task<LocalSession> CreateSessionAsync(
        LocalAccount account,
        string accessToken,
        string refreshToken,
        string idToken)
    {
        _logger.LogInformation("Creating session for user {Username}", account.Username);

        // Delete existing session if any (single session enforcement)
        var existingSession = await _dbContext.LocalSessions
            .FirstOrDefaultAsync(s => s.LocalAccountId == account.Id);

        if (existingSession != null)
        {
            _logger.LogInformation("Deleting existing session for user {Username}", account.Username);
            _dbContext.LocalSessions.Remove(existingSession);
            await _dbContext.SaveChangesAsync();
        }

        // Extract token expiration times
        var accessTokenExpiration = ExtractTokenExpiration(accessToken);
        var refreshTokenExpiration = ExtractTokenExpiration(refreshToken);

        // Create new session with encrypted tokens
        var session = new LocalSession
        {
            Id = Guid.NewGuid(),
            LocalAccountId = account.Id,
            AccessToken = _encryptionService.Encrypt(accessToken),
            RefreshToken = _encryptionService.Encrypt(refreshToken),
            IdToken = _encryptionService.Encrypt(idToken),
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration,
            CreatedAt = DateTime.UtcNow,
            LastAccessedAt = DateTime.UtcNow,
            IsActive = true
        };

        _dbContext.LocalSessions.Add(session);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Session created for user {Username}. Access token expires at {Expiration}",
            account.Username, accessTokenExpiration);

        return session;
    }

    /// <summary>
    /// Gets an active session by account ID
    /// </summary>
    public async Task<LocalSession?> GetSessionByAccountIdAsync(Guid accountId)
    {
        return await _dbContext.LocalSessions
            .Include(s => s.LocalAccount)
            .FirstOrDefaultAsync(s => s.LocalAccountId == accountId && s.IsActive);
    }

    /// <summary>
    /// Gets an active session by session ID
    /// </summary>
    public async Task<LocalSession?> GetSessionByIdAsync(Guid sessionId)
    {
        return await _dbContext.LocalSessions
            .Include(s => s.LocalAccount)
                .ThenInclude(a => a.LocalAccountRoles)
                    .ThenInclude(ar => ar.Role)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.IsActive);
    }

    /// <summary>
    /// Updates session with refreshed tokens.
    /// Used after successful token refresh from Keycloak.
    /// </summary>
    public async Task UpdateSessionTokensAsync(
        LocalSession session,
        string newAccessToken,
        string newRefreshToken)
    {
        var accessTokenExpiration = ExtractTokenExpiration(newAccessToken);
        var refreshTokenExpiration = ExtractTokenExpiration(newRefreshToken);

        session.AccessToken = _encryptionService.Encrypt(newAccessToken);
        session.RefreshToken = _encryptionService.Encrypt(newRefreshToken);
        session.AccessTokenExpiration = accessTokenExpiration;
        session.RefreshTokenExpiration = refreshTokenExpiration;
        session.LastAccessedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Session tokens updated for user {Username}. New expiration: {Expiration}",
            session.LocalAccount.Username, accessTokenExpiration);
    }

    /// <summary>
    /// Updates session last accessed time
    /// </summary>
    public async Task UpdateLastAccessedAsync(LocalSession session)
    {
        session.LastAccessedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a session (logout)
    /// </summary>
    public async Task DeleteSessionAsync(LocalSession session)
    {
        _logger.LogInformation("Deleting session for user {Username}", session.LocalAccount.Username);
        _dbContext.LocalSessions.Remove(session);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a session by account ID
    /// </summary>
    public async Task DeleteSessionByAccountIdAsync(Guid accountId)
    {
        var session = await GetSessionByAccountIdAsync(accountId);
        if (session != null)
        {
            await DeleteSessionAsync(session);
        }
    }

    /// <summary>
    /// Gets decrypted access token from session
    /// </summary>
    public string GetAccessToken(LocalSession session)
    {
        return _encryptionService.Decrypt(session.AccessToken);
    }

    /// <summary>
    /// Gets decrypted refresh token from session
    /// </summary>
    public string GetRefreshToken(LocalSession session)
    {
        return _encryptionService.Decrypt(session.RefreshToken);
    }

    /// <summary>
    /// Gets decrypted ID token from session
    /// </summary>
    public string GetIdToken(LocalSession session)
    {
        return _encryptionService.Decrypt(session.IdToken);
    }

    /// <summary>
    /// Checks if access token is expired
    /// </summary>
    public bool IsAccessTokenExpired(LocalSession session)
    {
        return session.AccessTokenExpiration <= DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if refresh token is expired
    /// </summary>
    public bool IsRefreshTokenExpired(LocalSession session)
    {
        return session.RefreshTokenExpiration <= DateTime.UtcNow;
    }

    /// <summary>
    /// Cleans up expired sessions (sessions where refresh token is expired).
    /// Called on login for prototype; will be moved to background job in V1.
    /// </summary>
    public async Task CleanupExpiredSessionsAsync()
    {
        var expiredSessions = await _dbContext.LocalSessions
            .Where(s => s.RefreshTokenExpiration <= DateTime.UtcNow)
            .ToListAsync();

        if (expiredSessions.Any())
        {
            _logger.LogInformation("Cleaning up {Count} expired sessions", expiredSessions.Count);
            _dbContext.LocalSessions.RemoveRange(expiredSessions);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extracts expiration time from JWT token.
    /// Returns DateTime.UtcNow + 1 hour if extraction fails.
    /// </summary>
    private DateTime ExtractTokenExpiration(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwt = handler.ReadJwtToken(token);
                if (jwt.ValidTo != DateTime.MinValue)
                {
                    return jwt.ValidTo;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract token expiration, using default");
        }

        // Default to 1 hour if we can't extract expiration
        return DateTime.UtcNow.AddHours(1);
    }
}
