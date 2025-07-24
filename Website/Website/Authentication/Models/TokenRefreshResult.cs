namespace Website.Authentication.Models;

public class TokenRefreshResult
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? IdToken { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    public static TokenRefreshResult Successful(string accessToken, string refreshToken, string? idToken = null, DateTime? expiresAt = null)
    {
        return new TokenRefreshResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IdToken = idToken,
            ExpiresAt = expiresAt
        };
    }
    
    public static TokenRefreshResult Failed(string errorMessage)
    {
        return new TokenRefreshResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}