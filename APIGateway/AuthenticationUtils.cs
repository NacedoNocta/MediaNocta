using System.Security.Claims;

namespace APIGateway;

// Extension methods for user role checking
public static class UserExtensions
{
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated == true)
            return false;

        // Check if user has the website-admin role
        // Roles are now properly mapped to standard role claims in JWT configuration
        return user.IsInRole("website-admin");
    }

    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true;
    }

    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst("sub")?.Value ?? "";
    }

    public static string GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst("preferred_username")?.Value ?? 
               user.FindFirst(ClaimTypes.Name)?.Value ?? "";
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst("email")?.Value ?? 
               user.FindFirst(ClaimTypes.Email)?.Value ?? "";
    }
}