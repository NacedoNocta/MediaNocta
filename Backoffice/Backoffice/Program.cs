using Backoffice.Components;
using Backoffice.Services;
using Backoffice.Authentication.Routes;
using Backoffice.Authentication.Extensions;
using Backoffice.Authentication.Middleware;
using Backoffice.Authentication.Handlers;
using Backoffice.Authentication.Providers;
using DatabaseManager.DbContexts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add auth database context (using backofficeDatabase - separate from Website)
builder.AddNpgsqlDbContext<AuthDbContext>("backofficeDatabase");

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

// FR-D5: Backoffice is exempt from caching entirely. Every outbound HttpClient call
// gets X-Skip-Cache: true so origin and gateway caches see it as a bypass request.
builder.Services.AddTransient<BackofficeBypassHandler>();

// Add cascading authentication state for all components
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Single Gateway endpoint for all API services
var gatewayUrl = builder.Configuration["GatewayEndpoint"] ?? throw new InvalidOperationException("GatewayEndpoint is not set");

// Configure service-specific clients with proper API paths and authenticated handler.
// BackofficeBypassHandler is wrapped outermost so X-Skip-Cache is set on every outbound call.
builder.Services.AddHttpClient<IBlogManagementService, BlogManagementService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/api/blog/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<BackofficeBypassHandler>();

builder.Services.AddHttpClient<ITechManagementService, TechManagementService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<BackofficeBypassHandler>();

builder.Services.AddHttpClient<IFragmentManagementService, FragmentManagementService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<BackofficeBypassHandler>();

builder.Services.AddHttpClient<IAuthorManagementService, AuthorManagementService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/api/blog/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<BackofficeBypassHandler>();

builder.Services.AddHttpClient<ICacheAdminService, CacheAdminService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/api/cache-admin/");
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<BackofficeBypassHandler>();

// Add a named HttpClient for authorized requests to the gateway
builder.Services.AddHttpClient("AuthorizedGateway", c =>
{
    c.BaseAddress = new Uri(gatewayUrl);
})
.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
.AddHttpMessageHandler<BackofficeBypassHandler>();

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

app.UseAuthentication();
app.UseTokenRefresh(); // Add token refresh middleware after authentication
app.UseAuthorization();

app.UseAntiforgery();

// Map static assets - allow anonymous access
app.MapStaticAssets()
    .AllowAnonymous();

// Add authentication endpoints (already configured with AllowAnonymous where needed)
app.MapAuthenticationEndpoints();

// Map Blazor components - will use fallback policy requiring authentication
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Backoffice.Client._Imports).Assembly);

app.Run();