using System.Security.Claims;
using Website.Authentication.Models;

namespace Website.Authentication.Interfaces;

public interface ISessionManager
{
    Task<UserSession?> GetCurrentSessionAsync();
    Task<bool> CreateSessionAsync(ClaimsPrincipal user, string accessToken, string refreshToken, string idToken);
    Task<bool> UpdateSessionAsync(ClaimsPrincipal user);
    Task<TokenRefreshResult> RefreshTokensAsync();
    Task<bool> IsValidSessionAsync();
    Task ClearSessionAsync();
    Task<string?> GetValidAccessTokenAsync();
    event EventHandler<UserSession?>? SessionChanged;
}