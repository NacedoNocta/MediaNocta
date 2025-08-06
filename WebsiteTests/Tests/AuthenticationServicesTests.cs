using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Website.Authentication.Interfaces;
using Website.Authentication.Models;
using Website.Authentication.Services;
using WebsiteTests.Infrastructure;
using AuthenticationOptions = Website.Authentication.Configuration.AuthenticationOptions;
using AuthenticationService = Microsoft.AspNetCore.Authentication.AuthenticationService;
using IAuthenticationService = Microsoft.AspNetCore.Authentication.IAuthenticationService;

namespace WebsiteTests.Tests;

public class AuthenticationServicesTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly SimpleMockKeycloakServer _mockKeycloak;

    public AuthenticationServicesTests()
    {
        _mockKeycloak = new SimpleMockKeycloakServer();
        
        var services = new ServiceCollection();
        
        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Authentication:Keycloak:ClientId", _mockKeycloak.ClientId},
                {"Authentication:Keycloak:ClientSecret", _mockKeycloak.ClientSecret},
                {"Authentication:Keycloak:Realm", _mockKeycloak.Realm},
                {"Authentication:Keycloak:RequireHttpsMetadata", "false"},
                {"ConnectionStrings:keycloak", _mockKeycloak.BaseUrl}
            })
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        
        // Add authentication services
        services.Configure<AuthenticationOptions>(options =>
        {
            options.Keycloak.ClientId = _mockKeycloak.ClientId;
            options.Keycloak.ClientSecret = _mockKeycloak.ClientSecret;
            options.Keycloak.Realm = _mockKeycloak.Realm;
            options.Keycloak.RequireHttpsMetadata = false;
        });
        
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        
        // Add authentication services
        services.AddScoped<ITokenRefreshService, TokenRefreshService>();
        services.AddScoped<SessionStore>();
        services.AddScoped<TokenUpdateQueue>();
        services.AddScoped<ISessionManager, SessionManager>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task TokenRefreshService_RefreshesToken_Successfully()
    {
        // Arrange
        var tokenRefreshService = _serviceProvider.GetRequiredService<ITokenRefreshService>();
        var validRefreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var result = await tokenRefreshService.RefreshAccessTokenAsync(validRefreshToken);

        // Assert
        result.Success.Should().BeTrue();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task TokenRefreshService_FailsWithInvalidToken()
    {
        // Arrange
        var tokenRefreshService = _serviceProvider.GetRequiredService<ITokenRefreshService>();
        var invalidRefreshToken = "invalid.token";

        // Act
        var result = await tokenRefreshService.RefreshAccessTokenAsync(invalidRefreshToken);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SessionManager_CreatesSession_ReturnsTrue()
    {
        // Arrange
        var sessionManager = _serviceProvider.GetRequiredService<ISessionManager>();
        var user = CreateTestClaimsPrincipal();
        var accessToken = _mockKeycloak.CreateValidAccessToken();
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var result = await sessionManager.CreateSessionAsync(user, accessToken, refreshToken, accessToken);

        // Assert - We can test that the method returns true, even if HttpContext functionality is limited
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SessionManager_SupportsTokenRefresh_Interface()
    {
        // Arrange
        var sessionManager = _serviceProvider.GetRequiredService<ISessionManager>();

        // Act & Assert - Test that the interface methods exist and can be called
        // (Even if they return expected results due to missing HttpContext)
        var isValid = await sessionManager.IsValidSessionAsync();
        var refreshResult = await sessionManager.RefreshTokensAsync();

        // These will be false/failed due to no HttpContext, but that's expected in unit tests
        isValid.Should().BeFalse();
        refreshResult.Success.Should().BeFalse();
    }

    [Fact]
    public async Task SessionManager_ClearsInvalidSessions()
    {
        // Arrange
        var sessionManager = _serviceProvider.GetRequiredService<ISessionManager>();
        var user = CreateTestClaimsPrincipal();
        var expiredAccessToken = _mockKeycloak.CreateExpiredAccessToken();
        var expiredRefreshToken = _mockKeycloak.CreateExpiredRefreshToken();

        await sessionManager.CreateSessionAsync(user, expiredAccessToken, expiredRefreshToken, expiredAccessToken);

        // Act
        var validToken = await sessionManager.GetValidAccessTokenAsync();

        // Assert
        validToken.Should().BeNull();
        
        var session = await sessionManager.GetCurrentSessionAsync();
        session.Should().BeNull();
    }

    [Fact]
    public void UserSession_ParsesTokensCorrectly()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var roles = new[] { "user", "admin" };
        var accessToken = _mockKeycloak.CreateValidAccessToken(roles: roles);
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var session = UserSession.FromClaimsPrincipal(user, accessToken, refreshToken, accessToken);

        // Assert
        session.UserId.Should().Be("test-user");
        session.UserName.Should().Be("testuser");
        session.Email.Should().Be("test@example.com");
        session.Roles.Should().Contain(roles);
        session.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UserSession_HandlesExpiredTokens()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var expiredAccessToken = _mockKeycloak.CreateExpiredAccessToken();
        var expiredRefreshToken = _mockKeycloak.CreateExpiredRefreshToken();

        // Act
        var session = UserSession.FromClaimsPrincipal(user, expiredAccessToken, expiredRefreshToken, expiredAccessToken);

        // Assert
        session.IsValid.Should().BeFalse();
        session.IsAccessTokenExpired.Should().BeTrue();
        session.IsRefreshTokenExpired.Should().BeTrue();
    }

    [Fact]
    public void UserSession_ValidWhenRefreshTokenActive()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var expiredAccessToken = _mockKeycloak.CreateExpiredAccessToken();
        var validRefreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var session = UserSession.FromClaimsPrincipal(user, expiredAccessToken, validRefreshToken, expiredAccessToken);

        // Assert
        session.IsValid.Should().BeTrue(); // Valid because refresh token can be used
        session.IsAccessTokenExpired.Should().BeTrue();
        session.IsRefreshTokenExpired.Should().BeFalse();
    }

    [Fact]
    public async Task TokenRevocation_WorksCorrectly()
    {
        // Arrange
        var tokenRefreshService = _serviceProvider.GetRequiredService<ITokenRefreshService>();
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var result = await tokenRefreshService.RevokeTokenAsync(refreshToken);

        // Assert
        result.Should().BeTrue();
    }

    private static ClaimsPrincipal CreateTestClaimsPrincipal(string userId = "test-user", string username = "testuser", string email = "test@example.com")
    {
        var claims = new[]
        {
            new Claim("sub", userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim("preferred_username", username),
            new Claim("email", email),
            new Claim(ClaimTypes.Email, email)
        };

        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }

    public void Dispose()
    {
        _mockKeycloak?.Dispose();
        _serviceProvider?.Dispose();
    }
}