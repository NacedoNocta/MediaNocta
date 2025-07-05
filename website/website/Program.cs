using Website.Components;
using Website.Services;
using Website.Authentication.Routes;
using Website.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add authentication services
builder.Services.AddCustomAuthentication(builder.Configuration);

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Single Gateway endpoint for all API services
var gatewayUrl = builder.Configuration["GatewayEndpoint"] ?? throw new InvalidOperationException("GatewayEndpoint is not set");

// Configure service-specific clients with proper API paths
builder.Services.AddHttpClient<BlogService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/api/blog/");
});

builder.Services.AddHttpClient<ActivityService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/api/activity/");
});

// Add a named HttpClient for authorized requests to the gateway
builder.Services.AddHttpClient("AuthorizedGateway", c =>
{
    c.BaseAddress = new Uri(gatewayUrl);
});

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
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

// Add authentication endpoints
app.MapAuthenticationEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Website.Client._Imports).Assembly);

app.Run();