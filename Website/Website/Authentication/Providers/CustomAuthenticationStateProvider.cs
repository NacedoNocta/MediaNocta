using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Website.Authentication.Services;
using System.Security.Claims;

namespace Website.Authentication.Providers;

/// <summary>
/// Custom AuthenticationStateProvider that extends Blazor's authentication state
/// with role information from the local database.
/// </summary>
public class CustomAuthenticationStateProvider : ServerAuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;

    public CustomAuthenticationStateProvider(
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider,
        ILogger<CustomAuthenticationStateProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Get base authentication state from ASP.NET Core
        var state = await base.GetAuthenticationStateAsync();

        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || !state.User.Identity?.IsAuthenticated == true)
        {
            return state;
        }

        try
        {
            // Get session and account information
            var sessionIdClaim = state.User.FindFirst("session_id")?.Value;
            var accountIdClaim = state.User.FindFirst("account_id")?.Value;

            if (string.IsNullOrEmpty(sessionIdClaim) || string.IsNullOrEmpty(accountIdClaim))
            {
                return state;
            }

            if (!Guid.TryParse(accountIdClaim, out var accountId))
            {
                return state;
            }

            // Create a scope to get scoped services
            using var scope = _serviceProvider.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountManagementService>();

            // Get account with roles
            var account = await accountService.GetAccountByIdAsync(accountId);

            if (account == null)
            {
                return state;
            }

            // Create new identity with role claims
            var identity = new ClaimsIdentity(state.User.Identity);

            // Add role claims from database
            foreach (var accountRole in account.LocalAccountRoles)
            {
                // Only add if not already present
                if (!state.User.HasClaim(ClaimTypes.Role, accountRole.Role.Name))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, accountRole.Role.Name));
                }
            }

            // Add additional user claims
            if (!state.User.HasClaim("username", account.Username))
            {
                identity.AddClaim(new Claim("username", account.Username));
            }

            if (!state.User.HasClaim("email", account.Email))
            {
                identity.AddClaim(new Claim("email", account.Email));
            }

            var enhancedPrincipal = new ClaimsPrincipal(identity);
            return new AuthenticationState(enhancedPrincipal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enhancing authentication state");
            return state;
        }
    }
}
