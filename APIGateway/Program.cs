var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer("keycloak", realm: "medianocta", options =>
    {
        options.Audience = "medianocta-api";
        options.RequireHttpsMetadata = false; // For development only
    });

builder.Services.AddAuthorization();

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

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/test/protected", () => "Protected endpoint accessed successfully!")
    .RequireAuthorization();

app.Run();