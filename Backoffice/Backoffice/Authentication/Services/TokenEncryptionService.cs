using Microsoft.AspNetCore.DataProtection;

namespace Backoffice.Authentication.Services;

/// <summary>
/// Service for encrypting and decrypting authentication tokens using ASP.NET Data Protection API.
/// Keys are configured from environment variables for security.
/// </summary>
public class TokenEncryptionService
{
    private readonly IDataProtector _protector;
    private readonly ILogger<TokenEncryptionService> _logger;
    private const string Purpose = "AuthenticationTokenProtection";

    public TokenEncryptionService(
        IDataProtectionProvider dataProtectionProvider,
        ILogger<TokenEncryptionService> logger)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
        _logger = logger;
    }

    /// <summary>
    /// Encrypts a token string for secure storage
    /// </summary>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            throw new ArgumentException("Cannot encrypt null or empty string", nameof(plainText));
        }

        try
        {
            return _protector.Protect(plainText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error encrypting token");
            throw;
        }
    }

    /// <summary>
    /// Decrypts an encrypted token string
    /// </summary>
    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            throw new ArgumentException("Cannot decrypt null or empty string", nameof(encryptedText));
        }

        try
        {
            return _protector.Unprotect(encryptedText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrypting token");
            throw;
        }
    }
}
