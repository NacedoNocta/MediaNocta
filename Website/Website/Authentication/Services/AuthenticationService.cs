using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Website.Authentication.Interfaces;
using IAuthenticationService = Website.Authentication.Interfaces.IAuthenticationService;

namespace Website.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IHttpContextAccessor httpContextAccessor,
        ISessionManager sessionManager,
        ILogger<AuthenticationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _sessionManager = sessionManager;
        _logger = logger;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await _sessionManager.GetValidAccessTokenAsync();
    }

    public async Task<ClaimsPrincipal?> GetUserAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            // Ensure session is up to date
            await _sessionManager.GetCurrentSessionAsync();
            return context.User;
        }
        return null;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        return await _sessionManager.IsValidSessionAsync();
    }

    public async Task SignInAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            var returnUrl = GetSafeReturnUrl(context.Request.Query["ReturnUrl"].FirstOrDefault());
            
            await context.ChallengeAsync("OpenIdConnect", new AuthenticationProperties 
            { 
                RedirectUri = returnUrl,
                IsPersistent = true
            });
        }
    }
    
    private static string GetSafeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || !returnUrl.StartsWith("/"))
        {
            return "/";
        }
        
        // Additional security: ensure URL doesn't contain protocol schemes
        if (returnUrl.Contains("://"))
        {
            return "/";
        }
        
        return returnUrl;
    }

    public async Task SignOutAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            // Clear local session first
            await _sessionManager.ClearSessionAsync();
            
            // Sign out from local cookie
            await context.SignOutAsync("Cookies");
            
            // Sign out from Keycloak (includes end session endpoint call)
            await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        }
    }
}