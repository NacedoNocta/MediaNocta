namespace APIGateway;

public static class UserContextMiddleware
{
    public static void UseUserContextHeaders(this WebApplication app)
    {
        // Custom middleware to add user context headers for downstream services
        app.Use(async (context, next) =>
        {
            if (context.User.IsAuthenticated())
            {
                // Add user identification headers for downstream services
                context.Request.Headers["X-User-Id"] = context.User.GetUserId();
                context.Request.Headers["X-User-Name"] = context.User.GetUsername();
                context.Request.Headers["X-User-Email"] = context.User.GetEmail();
                context.Request.Headers["X-User-Is-Admin"] = context.User.IsAdmin().ToString().ToLower();
                context.Request.Headers["X-User-Roles"] = context.User.FindFirst("realm_access")?.Value ?? "";
            }
            else
            {
                // Clear any user headers for non-authenticated requests
                context.Request.Headers.Remove("X-User-Id");
                context.Request.Headers.Remove("X-User-Name");
                context.Request.Headers.Remove("X-User-Email");
                context.Request.Headers.Remove("X-User-Is-Admin");
                context.Request.Headers.Remove("X-User-Roles");
            }
            
            await next();
        });
    }
}