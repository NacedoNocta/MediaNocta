using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    public IActionResult SetCulture(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions 
            { 
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            }
        );

        return LocalRedirect(returnUrl);
    }
}