using Microsoft.AspNetCore.Localization;

namespace Website.Services;

public class CultureService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public CultureService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string GetCurrentCulture()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return "en";

        var feature = httpContext.Features.Get<IRequestCultureFeature>();
        return feature?.RequestCulture?.Culture?.TwoLetterISOLanguageName ?? "en";
    }

    public void SetCulture(string culture)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
        httpContext.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            cookieValue,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });
    }

    public List<(string Code, string Name)> GetSupportedCultures()
    {
        return new List<(string Code, string Name)>
        {
            ("en", "English"),
            ("fr", "Français")
        };
    }
}