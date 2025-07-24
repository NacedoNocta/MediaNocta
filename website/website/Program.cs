using Website.Components;
using Website.Services;
using Website.Authentication.Routes;
using Website.Authentication.Extensions;
using Website.Authentication.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add authentication services
builder.Services.AddCustomAuthentication(builder.Configuration);

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

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

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add controllers for culture switching
builder.Services.AddControllers();

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

builder.Services.AddHttpClient<FragmentService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/");
});

builder.Services.AddHttpClient<TechUpdateService>(c =>
{
    c.BaseAddress = new Uri($"{gatewayUrl}/");
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

app.UseRequestLocalization();

app.UseAuthentication();
app.UseMiddleware<TokenUpdateMiddleware>();
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