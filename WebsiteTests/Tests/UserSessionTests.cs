using FluentAssertions;
using System.Security.Claims;
using Website.Authentication.Models;
using WebsiteTests.Infrastructure;

namespace WebsiteTests.Tests;

public class UserSessionTests : IDisposable
{
    private readonly SimpleMockKeycloakServer _mockKeycloak;

    public UserSessionTests()
    {
        _mockKeycloak = new SimpleMockKeycloakServer();
    }

    [Fact]
    public void UserSession_CreatesFromClaimsPrincipal_WithCorrectData()
    {
        // Arrange
        var userId = "test-user-123";
        var username = "testuser";
        var email = "test@example.com";
        var roles = new[] { "user", "admin" };
        
        var user = CreateTestClaimsPrincipal(userId, username, email);
        var accessToken = _mockKeycloak.CreateValidAccessToken(userId, username, email, roles);
        var refreshToken = _mockKeycloak.CreateValidRefreshToken(userId);
        var idToken = _mockKeycloak.CreateValidAccessToken(userId, username, email, roles);

        // Act
        var session = UserSession.FromClaimsPrincipal(user, accessToken, refreshToken, idToken);

        // Assert
        session.UserId.Should().Be(userId);
        session.UserName.Should().Be(username);
        session.Email.Should().Be(email);
        session.Roles.Should().Contain(roles);
        session.AccessToken.Should().Be(accessToken);
        session.RefreshToken.Should().Be(refreshToken);
        session.IdToken.Should().Be(idToken);
        session.SessionCreated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        session.LastActivity.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        session.IsKeycloakSessionExpired.Should().BeFalse();
    }

    [Fact]
    public void UserSession_ParsesTokenExpirations_Correctly()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var accessToken = _mockKeycloak.CreateValidAccessToken(expiration: DateTime.UtcNow.AddHours(1));
        var refreshToken = _mockKeycloak.CreateValidRefreshToken(expiration: DateTime.UtcNow.AddDays(30));

        // Act
        var session = UserSession.FromClaimsPrincipal(user, accessToken, refreshToken, accessToken);

        // Assert
        session.AccessTokenExpiration.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromMinutes(1));
        session.RefreshTokenExpiration.Should().BeCloseTo(DateTime.UtcNow.AddDays(30), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void UserSession_IsValid_ReturnsTrueWhenTokensValid()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var accessToken = _mockKeycloak.CreateValidAccessToken();
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var session = UserSession.FromClaimsPrincipal(user, accessToken, refreshToken, accessToken);

        // Assert
        session.IsValid.Should().BeTrue();
        session.IsAccessTokenExpired.Should().BeFalse();
        session.IsRefreshTokenExpired.Should().BeFalse();
    }

    [Fact]
    public void UserSession_IsValid_ReturnsFalseWhenBothTokensExpired()
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
    public void UserSession_IsValid_ReturnsTrueWhenRefreshTokenValid()
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
    public void UserSession_IsValid_ReturnsFalseWhenKeycloakSessionExpired()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var accessToken = _mockKeycloak.CreateValidAccessToken();
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();

        var session = UserSession.FromClaimsPrincipal(user, accessToken, refreshToken, accessToken);
        
        // Act
        session.IsKeycloakSessionExpired = true;

        // Assert
        session.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UserSession_UpdateTokens_UpdatesTokensAndMetadata()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var oldAccessToken = _mockKeycloak.CreateValidAccessToken();
        var oldRefreshToken = _mockKeycloak.CreateValidRefreshToken();
        
        var session = UserSession.FromClaimsPrincipal(user, oldAccessToken, oldRefreshToken, oldAccessToken);
        var originalLastActivity = session.LastActivity;
        
        // Wait a bit to ensure timestamp difference
        Thread.Sleep(10);
        
        var newAccessToken = _mockKeycloak.CreateValidAccessToken();
        var newRefreshToken = _mockKeycloak.CreateValidRefreshToken();
        var newIdToken = _mockKeycloak.CreateValidAccessToken();

        // Act
        session.UpdateTokens(newAccessToken, newRefreshToken, newIdToken);

        // Assert
        session.AccessToken.Should().Be(newAccessToken);
        session.RefreshToken.Should().Be(newRefreshToken);
        session.IdToken.Should().Be(newIdToken);
        session.LastActivity.Should().BeAfter(originalLastActivity);
    }

    [Fact]
    public void UserSession_UpdateFromClaimsPrincipal_UpdatesUserInfo()
    {
        // Arrange
        var originalUser = CreateTestClaimsPrincipal("original-user", "original", "original@example.com");
        var accessToken = _mockKeycloak.CreateValidAccessToken();
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();
        
        var session = UserSession.FromClaimsPrincipal(originalUser, accessToken, refreshToken, accessToken);
        var originalLastActivity = session.LastActivity;
        
        Thread.Sleep(10);
        
        var updatedUser = CreateTestClaimsPrincipal("updated-user", "updated", "updated@example.com");

        // Act
        session.UpdateFromClaimsPrincipal(updatedUser);

        // Assert
        session.UserId.Should().Be("updated-user");
        session.UserName.Should().Be("updated");
        session.Email.Should().Be("updated@example.com");
        session.LastActivity.Should().BeAfter(originalLastActivity);
    }

    [Fact]
    public void UserSession_ExtractsRolesFromJWT_Correctly()
    {
        // Arrange
        var roles = new[] { "user", "admin", "moderator" };
        var user = CreateTestClaimsPrincipal();
        var accessToken = _mockKeycloak.CreateValidAccessToken(roles: roles);
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var session = UserSession.FromClaimsPrincipal(user, accessToken, refreshToken, accessToken);

        // Assert
        session.Roles.Should().Contain(roles);
        session.Roles.Should().HaveCount(roles.Length);
    }

    [Fact]
    public void UserSession_HandlesEmptyRoles_Gracefully()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var accessToken = _mockKeycloak.CreateValidAccessToken(roles: Array.Empty<string>());
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var session = UserSession.FromClaimsPrincipal(user, accessToken, refreshToken, accessToken);

        // Assert
        session.Roles.Should().NotBeNull();
        session.Roles.Should().BeEmpty();
    }

    [Fact]
    public void UserSession_HandlesInvalidJWT_Gracefully()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var invalidToken = "invalid.jwt.token";
        var refreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        var session = UserSession.FromClaimsPrincipal(user, invalidToken, refreshToken, invalidToken);

        // Assert
        session.Should().NotBeNull();
        session.AccessToken.Should().Be(invalidToken);
        session.Roles.Should().BeEmpty(); // Should handle parsing gracefully
        // Should set default expiration times
        session.AccessTokenExpiration.Should().BeAfter(DateTime.UtcNow);
        session.RefreshTokenExpiration.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void UserSession_UpdateTokens_KeepsIdTokenWhenNotProvided()
    {
        // Arrange
        var user = CreateTestClaimsPrincipal();
        var oldAccessToken = _mockKeycloak.CreateValidAccessToken();
        var oldRefreshToken = _mockKeycloak.CreateValidRefreshToken();
        var oldIdToken = _mockKeycloak.CreateValidAccessToken();
        
        var session = UserSession.FromClaimsPrincipal(user, oldAccessToken, oldRefreshToken, oldIdToken);
        
        var newAccessToken = _mockKeycloak.CreateValidAccessToken();
        var newRefreshToken = _mockKeycloak.CreateValidRefreshToken();

        // Act
        session.UpdateTokens(newAccessToken, newRefreshToken); // No ID token provided

        // Assert
        session.AccessToken.Should().Be(newAccessToken);
        session.RefreshToken.Should().Be(newRefreshToken);
        session.IdToken.Should().Be(oldIdToken); // Should keep the old ID token
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
    }
}