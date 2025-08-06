using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WebsiteTests.Infrastructure;

public class SimpleMockKeycloakServer : IDisposable
{
    private readonly WireMockServer _server;
    private readonly RSA _rsa;
    private readonly RsaSecurityKey _signingKey;
    private readonly string _kid = "test-key-id";
    
    public string BaseUrl => _server.Url!;
    public string Realm { get; } = "test-realm";
    public string ClientId { get; } = "test-client";
    public string ClientSecret { get; } = "test-secret";

    public SimpleMockKeycloakServer()
    {
        _server = WireMockServer.Start();
        _rsa = RSA.Create(2048);
        _signingKey = new RsaSecurityKey(_rsa) { KeyId = _kid };
        
        SetupEndpoints();
    }

    private void SetupEndpoints()
    {
        // Well-known configuration endpoint
        var wellKnownConfig = new
        {
            issuer = $"{BaseUrl}/realms/{Realm}",
            authorization_endpoint = $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/auth",
            token_endpoint = $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/token",
            userinfo_endpoint = $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/userinfo",
            end_session_endpoint = $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/logout",
            jwks_uri = $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/certs",
            revocation_endpoint = $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/revoke",
            response_types_supported = new[] { "code", "id_token", "token" },
            grant_types_supported = new[] { "authorization_code", "refresh_token" },
            subject_types_supported = new[] { "public" },
            id_token_signing_alg_values_supported = new[] { "RS256" },
            scopes_supported = new[] { "openid", "profile", "email", "roles" }
        };

        _server
            .Given(Request.Create().WithPath($"/realms/{Realm}/.well-known/openid_configuration").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(wellKnownConfig));

        // JWKS endpoint
        var jwks = new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA",
                    use = "sig",
                    kid = _kid,
                    n = Convert.ToBase64String(_rsa.ExportParameters(false).Modulus!),
                    e = Convert.ToBase64String(_rsa.ExportParameters(false).Exponent!),
                    alg = "RS256"
                }
            }
        };

        _server
            .Given(Request.Create().WithPath($"/realms/{Realm}/protocol/openid-connect/certs").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(jwks));

        // Token endpoint - successful response
        var tokenResponse = new
        {
            access_token = CreateValidAccessToken(),
            refresh_token = CreateValidRefreshToken(),
            id_token = CreateValidAccessToken(),
            token_type = "Bearer",
            expires_in = 3600,
            refresh_expires_in = 2592000
        };

        _server
            .Given(Request.Create()
                .WithPath($"/realms/{Realm}/protocol/openid-connect/token")
                .UsingPost()
                .WithBody("*grant_type=authorization_code*")
                .WithBody("*client_id=test-client*"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(tokenResponse));

        // Token endpoint - refresh token success
        var refreshResponse = new
        {
            access_token = CreateValidAccessToken(),
            refresh_token = CreateValidRefreshToken(),
            token_type = "Bearer",
            expires_in = 3600,
            refresh_expires_in = 2592000
        };

        _server
            .Given(Request.Create()
                .WithPath($"/realms/{Realm}/protocol/openid-connect/token")
                .UsingPost()
                .WithBody("*grant_type=refresh_token*")
                .WithBody("*client_id=test-client*"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(refreshResponse));

        // Token endpoint - expired refresh token
        _server
            .Given(Request.Create()
                .WithPath($"/realms/{Realm}/protocol/openid-connect/token")
                .UsingPost()
                .WithBody("*grant_type=refresh_token*")
                .WithBody("*refresh_token=*expired*"))
            .RespondWith(Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { error = "invalid_grant", error_description = "Session not active" }));

        // Token endpoint - invalid refresh token
        _server
            .Given(Request.Create()
                .WithPath($"/realms/{Realm}/protocol/openid-connect/token")
                .UsingPost()
                .WithBody("*grant_type=refresh_token*")
                .WithBody("*refresh_token=*invalid*"))
            .RespondWith(Response.Create()
                .WithStatusCode(400)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { error = "invalid_grant", error_description = "Invalid refresh token" }));

        // Revoke endpoint
        _server
            .Given(Request.Create()
                .WithPath($"/realms/{Realm}/protocol/openid-connect/revoke")
                .UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200));

        // UserInfo endpoint
        var userInfo = new
        {
            sub = "test-user-id",
            name = "Test User",
            preferred_username = "testuser",
            email = "test@example.com",
            email_verified = true,
            realm_access = new
            {
                roles = new[] { "user", "admin" }
            }
        };

        _server
            .Given(Request.Create()
                .WithPath($"/realms/{Realm}/protocol/openid-connect/userinfo")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(userInfo));
    }

    public string CreateValidAccessToken(string userId = "test-user-id", string username = "testuser", 
        string email = "test@example.com", string[] roles = null, DateTime? expiration = null)
    {
        roles ??= new[] { "user" };
        expiration ??= DateTime.UtcNow.AddHours(1);

        var claims = new[]
        {
            new Claim("sub", userId),
            new Claim("preferred_username", username),
            new Claim("email", email),
            new Claim("email_verified", "true"),
            new Claim("iss", $"{BaseUrl}/realms/{Realm}"),
            new Claim("aud", ClientId),
            new Claim("typ", "Bearer"),
            new Claim("azp", ClientId)
        };

        var realmAccess = new
        {
            roles = roles
        };
        
        var allClaims = claims.ToList();
        allClaims.Add(new Claim("realm_access", JsonSerializer.Serialize(realmAccess), JsonClaimValueTypes.Json));

        var tokenHandler = new JwtSecurityTokenHandler();
        var notBefore = expiration < DateTime.UtcNow 
            ? expiration.Value.AddMinutes(-30) // For expired tokens, set NotBefore well before expiry
            : DateTime.UtcNow.AddMinutes(-5);   // For valid tokens, set NotBefore to recent past

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(allClaims),
            Expires = expiration,
            NotBefore = notBefore,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256),
            Issuer = $"{BaseUrl}/realms/{Realm}",
            Audience = ClientId
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string CreateValidRefreshToken(string userId = "test-user-id", DateTime? expiration = null)
    {
        expiration ??= DateTime.UtcNow.AddDays(30);

        var claims = new[]
        {
            new Claim("sub", userId),
            new Claim("typ", "Refresh"),
            new Claim("iss", $"{BaseUrl}/realms/{Realm}"),
            new Claim("aud", ClientId)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var notBefore = expiration < DateTime.UtcNow 
            ? expiration.Value.AddMinutes(-30) // For expired tokens, set NotBefore well before expiry
            : DateTime.UtcNow.AddMinutes(-5);   // For valid tokens, set NotBefore to recent past

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            NotBefore = notBefore,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256),
            Issuer = $"{BaseUrl}/realms/{Realm}",
            Audience = ClientId
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string CreateExpiredAccessToken(string userId = "test-user-id")
    {
        return CreateValidAccessToken(userId, expiration: DateTime.UtcNow.AddMinutes(-10));
    }

    public string CreateExpiredRefreshToken(string userId = "test-user-id")
    {
        return CreateValidRefreshToken(userId, expiration: DateTime.UtcNow.AddMinutes(-10));
    }

    public void Dispose()
    {
        _server?.Stop();
        _server?.Dispose();
        _rsa?.Dispose();
    }
}