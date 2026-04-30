using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using IAuthenticationService = Backoffice.Authentication.Interfaces.IAuthenticationService;

namespace Backoffice.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            var token = await context.GetTokenAsync("access_token");
            _logger.LogDebug("Access token retrieved: {TokenExists}", !string.IsNullOrEmpty(token));
            return token;
        }
        _logger.LogDebug("User not authenticated or no context available");
        return null;
    }

    public async Task<ClaimsPrincipal?> GetUserAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        return await Task.FromResult(context?.User);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        return await Task.FromResult(context?.User?.Identity?.IsAuthenticated == true);
    }

    public async Task SignInAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            var returnUrl = context.Request.Query["ReturnUrl"].FirstOrDefault() ?? "/";
            await context.ChallengeAsync("OpenIdConnect", new AuthenticationProperties 
            { 
                RedirectUri = returnUrl,
                IsPersistent = true
            });
        }
    }

    public async Task SignOutAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            await context.SignOutAsync("Cookies");
            await context.SignOutAsync("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        }
    }
}