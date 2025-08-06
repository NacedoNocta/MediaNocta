using APIGateway;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

// Add Keycloak authentication and authorization
builder.Services.AddKeycloakAuthentication(builder.Configuration);
builder.Services.AddKeycloakAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.AddServiceDefaults();

var app = builder.Build();

// Configure middleware pipeline
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Add user context headers for downstream services
app.UseUserContextHeaders();

// Configure routing
app.MapReverseProxy();
app.MapTestEndpoints();

app.Run();