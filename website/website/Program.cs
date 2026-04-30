using Website.Components;
using Website.Services;
using Website.Authentication.Routes;
using Website.Authentication.Extensions;
using Website.Authentication.Middleware;
using Website.Authentication.Handlers;
using Website.Authentication.Providers;
using CacheLibrary;
using DatabaseManager;
using DatabaseManager.DbContexts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add auth database context (using websiteDatabase - separate from APIs)
builder.AddNpgsqlDbContext<AuthDbContext>("websiteDatabase");

// Add authentication services
builder.Services.AddCustomAuthentication(builder.Configuration);

// Register custom authentication state provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<ServerAuthenticationStateProvider>(sp =>
    sp.GetRequiredService<AuthenticationStateProvider>() as ServerAuthenticationStateProvider
    ?? throw new InvalidOperationException("AuthenticationStateProvider is not a ServerAuthenticationStateProvider"));

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Register authenticated HTTP client handler
builder.Services.AddTransient<AuthenticatedHttpClientHandler>();

// Redis-backed distributed cache (FR-D1) and the per-method opt-in caching delegating handler.
// Fail-fast SE.Redis config + 50ms hard wrapper in WebsiteCachingHandler = total ~50ms cache-down budget.
builder.AddRedisDistributedCache("cache", configureOptions: CacheRedisDefaults.ApplyFailFast);
builder.Services.AddCacheLibrary();
builder.Services.AddTransient<WebsiteCachingHandler>();

// Add localization services
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "fr" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

// Add culture service
builder.Services.AddScoped<CultureService>();

// Add cascading authentication state for all components
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add controllers for culture switching
builder.Services.AddControllers();

// Single Gateway endpoint for all API services
var gatewayUrl = builder.Configuration["GatewayEndpoint"] ?? throw new InvalidOperationException("GatewayEndpoint is not set");

// Configure service-specific clients with proper API paths.
// Order matters: AuthenticatedHttpClientHandler is innermost; WebsiteCachingHandler wraps it
// so cached responses bypass the auth handler entirely on hits.
builder.Services.AddHttpClient<BlogService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/api/blog/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<WebsiteCachingHandler>();

builder.Services.AddHttpClient<ActivityService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/api/activity/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<WebsiteCachingHandler>();

builder.Services.AddHttpClient<FragmentService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<WebsiteCachingHandler>();

builder.Services.AddHttpClient<TechUpdateService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<WebsiteCachingHandler>();

// Add a named HttpClient for authorized requests to the gateway
builder.Services.AddHttpClient("AuthorizedGateway", c =>
{
    c.BaseAddress = new Uri(gatewayUrl);
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseAuthentication();
app.UseTokenRefresh(); // Add token refresh middleware after authentication
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

// Add authentication endpoints
app.MapAuthenticationEndpoints();

// Map controllers for culture switching
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Website.Client._Imports).Assembly);

app.Run();