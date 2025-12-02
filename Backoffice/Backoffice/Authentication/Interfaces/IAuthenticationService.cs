using System.Security.Claims;

namespace Backoffice.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<string?> GetAccessTokenAsync();
    Task<ClaimsPrincipal?> GetUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task SignInAsync();
    Task SignOutAsync();
}