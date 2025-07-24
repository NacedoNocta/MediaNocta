using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Website.Authentication.Configuration;
using Website.Authentication.Interfaces;
using Website.Authentication.Models;

namespace Website.Authentication.Services;

public class TokenRefreshService : ITokenRefreshService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationOptions _authOptions;
    private readonly ILogger<TokenRefreshService> _logger;
    private readonly string _keycloakTokenEndpoint;

    public TokenRefreshService(
        HttpClient httpClient,
        IOptions<AuthenticationOptions> authOptions,
        ILogger<TokenRefreshService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _authOptions = authOptions.Value;
        _logger = logger;
        
        // Build Keycloak token endpoint URL
        var keycloakBaseUrl = configuration.GetConnectionString("keycloak") ?? "http://keycloak:8080";
        _keycloakTokenEndpoint = $"{keycloakBaseUrl}/realms/{_authOptions.Keycloak.Realm}/protocol/openid-connect/token";
        
        _logger.LogDebug("TokenRefreshService initialized with endpoint: {Endpoint}", _keycloakTokenEndpoint);
    }

    public async Task<TokenRefreshResult> RefreshAccessTokenAsync(string refreshToken)
    {
        try
        {
            _logger.LogDebug("Attempting to refresh access token");

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", _authOptions.Keycloak.ClientId),
                new KeyValuePair<string, string>("client_secret", _authOptions.Keycloak.ClientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            });

            var response = await _httpClient.PostAsync(_keycloakTokenEndpoint, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                var accessToken = tokenResponse.GetProperty("access_token").GetString();
                var newRefreshToken = tokenResponse.TryGetProperty("refresh_token", out var refreshProp) 
                    ? refreshProp.GetString() 
                    : refreshToken; // Keep existing refresh token if not returned
                var idToken = tokenResponse.TryGetProperty("id_token", out var idProp) 
                    ? idProp.GetString() 
                    : null;

                DateTime? expiresAt = null;
                if (tokenResponse.TryGetProperty("expires_in", out var expiresInProp))
                {
                    var expiresIn = expiresInProp.GetInt32();
                    expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
                }

                _logger.LogDebug("Token refresh successful");
                return TokenRefreshResult.Successful(accessToken!, newRefreshToken!, idToken, expiresAt);
            }
            else
            {
                // Parse error details for better error handling
                string errorType = "unknown";
                string errorMessage = $"Token refresh failed: {response.StatusCode}";
                
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    if (errorResponse.TryGetProperty("error", out var errorProp))
                    {
                        errorType = errorProp.GetString() ?? "unknown";
                    }
                    if (errorResponse.TryGetProperty("error_description", out var errorDescProp))
                    {
                        var errorDesc = errorDescProp.GetString();
                        errorMessage = $"{errorType}: {errorDesc}";
                    }
                }
                catch
                {
                    // Fall back to simple error message
                }

                _logger.LogWarning("Token refresh failed with status {StatusCode} and error {ErrorType}: {Response}", 
                    response.StatusCode, errorType, responseContent);
                
                // Specific handling for session expiry
                if (errorType == "invalid_grant" && responseContent.Contains("Session not active"))
                {
                    return TokenRefreshResult.Failed("KEYCLOAK_SESSION_EXPIRED");
                }
                
                return TokenRefreshResult.Failed(errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during token refresh");
            return TokenRefreshResult.Failed($"Token refresh exception: {ex.Message}");
        }
    }

    public async Task<bool> RevokeTokenAsync(string token, string tokenType = "refresh_token")
    {
        try
        {
            _logger.LogDebug("Attempting to revoke {TokenType} token", tokenType);

            var revokeEndpoint = _keycloakTokenEndpoint.Replace("/token", "/revoke");
            
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _authOptions.Keycloak.ClientId),
                new KeyValuePair<string, string>("client_secret", _authOptions.Keycloak.ClientSecret),
                new KeyValuePair<string, string>("token", token),
                new KeyValuePair<string, string>("token_type_hint", tokenType)
            });

            var response = await _httpClient.PostAsync(revokeEndpoint, requestContent);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Token revocation successful");
                return true;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Token revocation failed with status {StatusCode}: {Response}", response.StatusCode, responseContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during token revocation");
            return false;
        }
    }
}