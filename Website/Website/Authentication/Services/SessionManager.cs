using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Website.Authentication.Interfaces;
using Website.Authentication.Models;

namespace Website.Authentication.Services;

public class SessionManager : ISessionManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenRefreshService _tokenRefreshService;
    private readonly TokenUpdateQueue _tokenUpdateQueue;
    private readonly SessionStore _sessionStore;
    private readonly ILogger<SessionManager> _logger;
    private UserSession? _currentSession;
    
    public event EventHandler<UserSession?>? SessionChanged;

    public SessionManager(
        IHttpContextAccessor httpContextAccessor,
        ITokenRefreshService tokenRefreshService,
        TokenUpdateQueue tokenUpdateQueue,
        SessionStore sessionStore,
        ILogger<SessionManager> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenRefreshService = tokenRefreshService;
        _tokenUpdateQueue = tokenUpdateQueue;
        _sessionStore = sessionStore;
        _logger = logger;
    }

    public async Task<UserSession?> GetCurrentSessionAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated != true)
        {
            await ClearSessionAsync();
            return null;
        }

        // If we have a cached session, validate it
        if (_currentSession != null)
        {
            if (!_currentSession.IsValid)
            {
                _logger.LogDebug("Cached session is invalid, clearing");
                await ClearSessionAsync();
                return null;
            }

            // Update last activity
            _currentSession.LastActivity = DateTime.UtcNow;
            return _currentSession;
        }

        // Create session from current authentication context
        try
        {
            var accessToken = await context.GetTokenAsync("access_token");
            var refreshToken = await context.GetTokenAsync("refresh_token");
            var idToken = await context.GetTokenAsync("id_token");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Missing tokens in authentication context");
                await ClearSessionAsync();
                return null;
            }

            _currentSession = UserSession.FromClaimsPrincipal(context.User, accessToken, refreshToken, idToken ?? string.Empty);
            _logger.LogDebug("Session created for user: {UserId}", _currentSession.UserId);
            
            // Store session in the session store for background access
            _sessionStore.StoreSession(_currentSession);
            
            SessionChanged?.Invoke(this, _currentSession);
            return _currentSession;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session from authentication context");
            await ClearSessionAsync();
            return null;
        }
    }

    public async Task<bool> CreateSessionAsync(ClaimsPrincipal user, string accessToken, string refreshToken, string idToken)
    {
        try
        {
            _currentSession = UserSession.FromClaimsPrincipal(user, accessToken, refreshToken, idToken);
            _logger.LogInformation("Session created for user: {UserId}", _currentSession.UserId);
            
            // Store session in the session store for background access
            _sessionStore.StoreSession(_currentSession);
            
            SessionChanged?.Invoke(this, _currentSession);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session");
            return false;
        }
    }

    public Task<bool> UpdateSessionAsync(ClaimsPrincipal user)
    {
        if (_currentSession == null)
        {
            return Task.FromResult(false);
        }

        try
        {
            _currentSession.UpdateFromClaimsPrincipal(user);
            _logger.LogDebug("Session updated for user: {UserId}", _currentSession.UserId);
            
            // Update session in the session store
            _sessionStore.StoreSession(_currentSession);
            
            SessionChanged?.Invoke(this, _currentSession);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating session");
            return Task.FromResult(false);
        }
    }

    public async Task<TokenRefreshResult> RefreshTokensAsync()
    {
        if (_currentSession == null || string.IsNullOrEmpty(_currentSession.RefreshToken))
        {
            _logger.LogWarning("No session or refresh token available for refresh");
            return TokenRefreshResult.Failed("No session or refresh token available");
        }

        if (_currentSession.IsRefreshTokenExpired)
        {
            _logger.LogWarning("Refresh token is expired, session must be recreated");
            await ClearSessionAsync();
            return TokenRefreshResult.Failed("Refresh token expired");
        }

        try
        {
            var result = await _tokenRefreshService.RefreshAccessTokenAsync(_currentSession.RefreshToken);
            
            if (result.Success && result.AccessToken != null && result.RefreshToken != null)
            {
                _currentSession.UpdateTokens(result.AccessToken, result.RefreshToken, result.IdToken);
                
                // Update session in the session store
                _sessionStore.StoreSession(_currentSession);
                
                // Queue the token update for the next request cycle
                _tokenUpdateQueue.QueueTokenUpdate(_currentSession.UserId, result.AccessToken, result.RefreshToken, result.IdToken);
                
                _logger.LogInformation("Tokens refreshed successfully for user: {UserId}", _currentSession.UserId);
                SessionChanged?.Invoke(this, _currentSession);
            }
            else
            {
                _logger.LogWarning("Token refresh failed: {ErrorMessage}", result.ErrorMessage);
                
                // Check if this is a Keycloak session expiry
                if (result.ErrorMessage == "KEYCLOAK_SESSION_EXPIRED")
                {
                    _logger.LogInformation("Keycloak session expired for user: {UserId}, marking session as expired", _currentSession.UserId);
                    _currentSession.IsKeycloakSessionExpired = true;
                    
                    // Update session in the session store to reflect Keycloak session expiry
                    _sessionStore.StoreSession(_currentSession);
                    
                    SessionChanged?.Invoke(this, _currentSession);
                    await ForceLogoutAsync();
                }
                else
                {
                    await ClearSessionAsync();
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during token refresh");
            await ClearSessionAsync();
            return TokenRefreshResult.Failed($"Token refresh exception: {ex.Message}");
        }
    }

    public async Task<bool> IsValidSessionAsync()
    {
        var session = await GetCurrentSessionAsync();
        return session?.IsValid == true;
    }

    public async Task ClearSessionAsync()
    {
        if (_currentSession != null)
        {
            _logger.LogInformation("Clearing session for user: {UserId}", _currentSession.UserId);
            
            // Remove session from the session store
            _sessionStore.RemoveSession(_currentSession.UserId);
            
            // Clear any pending token updates for this user
            _tokenUpdateQueue.ClearUpdatesForUser(_currentSession.UserId);
            
            // Attempt to revoke refresh token
            try
            {
                if (!string.IsNullOrEmpty(_currentSession.RefreshToken))
                {
                    await _tokenRefreshService.RevokeTokenAsync(_currentSession.RefreshToken, "refresh_token");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to revoke refresh token during session clear");
            }
        }

        _currentSession = null;
        SessionChanged?.Invoke(this, null);
    }

    private async Task ForceLogoutAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        
        // Clear pending token updates for this user
        if (_currentSession != null)
        {
            _tokenUpdateQueue.ClearUpdatesForUser(_currentSession.UserId);
        }
        
        // Clear local session first
        _currentSession = null;
        SessionChanged?.Invoke(this, null);
        
        if (context != null)
        {
            try
            {
                // Sign out from local cookie
                await context.SignOutAsync("Cookies");
                
                // Note: We don't try to sign out from Keycloak since the session is already expired there
                _logger.LogDebug("Local authentication cleared due to expired Keycloak session");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during force logout");
            }
        }
    }

    public async Task<string?> GetValidAccessTokenAsync()
    {
        var session = await GetCurrentSessionAsync();
        if (session == null)
        {
            return null;
        }

        // If access token is still valid, return it
        if (!session.IsAccessTokenExpired)
        {
            return session.AccessToken;
        }

        // Try to refresh the token
        var refreshResult = await RefreshTokensAsync();
        if (refreshResult.Success)
        {
            return refreshResult.AccessToken;
        }

        // Refresh failed, clear session
        await ClearSessionAsync();
        return null;
    }

}