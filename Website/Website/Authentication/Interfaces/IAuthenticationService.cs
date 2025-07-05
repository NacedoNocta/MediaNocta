using System.Security.Claims;

namespace Website.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<string?> GetAccessTokenAsync();
    Task<ClaimsPrincipal?> GetUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task SignInAsync();
    Task SignOutAsync();
}