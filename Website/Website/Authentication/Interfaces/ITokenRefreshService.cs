using Website.Authentication.Models;

namespace Website.Authentication.Interfaces;

public interface ITokenRefreshService
{
    Task<TokenRefreshResult> RefreshAccessTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string token, string tokenType = "refresh_token");
}