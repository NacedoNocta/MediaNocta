using CacheAdminApi.Services;
using CacheLibrary;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Aspire StackExchange.Redis client integration — provides IConnectionMultiplexer
// for raw Redis access (SCAN/DEL operations). Fail-fast settings prevent admin UI hangs when Redis is down.
builder.AddRedisClient("cache", configureOptions: CacheRedisDefaults.ApplyFailFast);
builder.Services.AddCacheLibrary();
builder.Services.AddSingleton<IRedisInspector, RedisInspector>();

// Keycloak authentication, mirroring the APIGateway pattern (FR-F7).
builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer("keycloak", realm: "medianocta", options =>
    {
        options.Audience = "medianocta-api";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.RoleClaimType = "role";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireAuthenticatedUser().RequireRole("website-admin"));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
