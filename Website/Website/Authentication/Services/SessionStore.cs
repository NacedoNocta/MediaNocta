using System.Collections.Concurrent;
using Website.Authentication.Models;

namespace Website.Authentication.Services;

/// <summary>
/// Thread-safe store for managing user sessions across different contexts
/// including background services that don't have access to HTTP context
/// </summary>
public class SessionStore
{
    private readonly ConcurrentDictionary<string, UserSession> _sessions = new();
    private readonly ILogger<SessionStore> _logger;
    
    public SessionStore(ILogger<SessionStore> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Stores or updates a user session
    /// </summary>
    public void StoreSession(UserSession session)
    {
        if (session == null || string.IsNullOrEmpty(session.UserId))
        {
            _logger.LogWarning("Attempted to store null or invalid session");
            return;
        }
        
        _sessions.AddOrUpdate(session.UserId, session, (key, oldValue) => session);
        _logger.LogDebug("Session stored for user: {UserId}", session.UserId);
    }
    
    /// <summary>
    /// Retrieves a session by user ID
    /// </summary>
    public UserSession? GetSession(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }
        
        _sessions.TryGetValue(userId, out var session);
        return session;
    }
    
    /// <summary>
    /// Gets all active sessions that are still valid
    /// </summary>
    public IEnumerable<UserSession> GetActiveSessions()
    {
        var activeSessions = new List<UserSession>();
        
        foreach (var kvp in _sessions)
        {
            var session = kvp.Value;
            
            // Remove expired sessions
            if (!session.IsValid)
            {
                _sessions.TryRemove(kvp.Key, out _);
                _logger.LogDebug("Removed expired session for user: {UserId}", kvp.Key);
                continue;
            }
            
            activeSessions.Add(session);
        }
        
        return activeSessions;
    }
    
    /// <summary>
    /// Updates session tokens
    /// </summary>
    public bool UpdateSessionTokens(string userId, string accessToken, string refreshToken, string? idToken = null)
    {
        if (_sessions.TryGetValue(userId, out var session))
        {
            session.UpdateTokens(accessToken, refreshToken, idToken);
            _logger.LogDebug("Updated tokens for session: {UserId}", userId);
            return true;
        }
        
        _logger.LogWarning("Attempted to update tokens for non-existent session: {UserId}", userId);
        return false;
    }
    
    /// <summary>
    /// Removes a session from the store
    /// </summary>
    public bool RemoveSession(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }
        
        var removed = _sessions.TryRemove(userId, out var session);
        
        if (removed && session != null)
        {
            _logger.LogInformation("Session removed for user: {UserId}", userId);
        }
        
        return removed;
    }
    
    /// <summary>
    /// Marks a session as having an expired Keycloak session
    /// </summary>
    public bool MarkKeycloakSessionExpired(string userId)
    {
        if (_sessions.TryGetValue(userId, out var session))
        {
            session.IsKeycloakSessionExpired = true;
            _logger.LogDebug("Marked Keycloak session as expired for user: {UserId}", userId);
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Gets the count of active sessions
    /// </summary>
    public int ActiveSessionCount => _sessions.Count;
    
    /// <summary>
    /// Clears all sessions (useful for testing or application shutdown)
    /// </summary>
    public void ClearAllSessions()
    {
        var count = _sessions.Count;
        _sessions.Clear();
        _logger.LogInformation("Cleared all sessions. Total cleared: {Count}", count);
    }
}