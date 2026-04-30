using TechAPI.Interfaces;
using TechAPI.Services;
using TechAPI.Repositories;
using CacheLibrary;
using DatabaseManager;
using DatabaseManager.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("mainDatabase");
    options.UseNpgsql(connectionString);
});

// Add services to the container.
builder.Services.AddScoped<ITechUpdateRepository, EfTechUpdateRepository>();
builder.Services.AddScoped<ITechUpdateService, TechUpdateService>();

// Redis-backed output caching with named per-resource policies (24h TTL, tag-based eviction).
// Base policy honours the X-Skip-Cache bypass header (FR-D6).
builder.AddRedisOutputCache("cache", configureOptions: CacheRedisDefaults.ApplyFailFast);
builder.Services.AddCacheBypass(CacheNamespaces.Main, CacheProducers.TechApi);
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.AddPolicy<BypassHeaderPolicy>());

    var defaultTtl = TimeSpan.FromDays(1);
    options.AddPolicy("TechUpdates", b => b
        .Tag(CacheTags.TechApi.Updates)
        .SetVaryByQuery("projectId", "page", "pageSize")
        .Expire(defaultTtl));
    options.AddPolicy("TechUpdatesCount", b => b
        .Tag(CacheTags.TechApi.UpdatesCount)
        .SetVaryByQuery("projectId")
        .Expire(defaultTtl));
    options.AddPolicy("TechUpdateDetail", b => b
        .Tag(CacheTags.TechApi.UpdateDetailAll)
        .SetVaryByRouteValue("id")
        .Expire(defaultTtl));
    options.AddPolicy("TechProjectActivity", b => b
        .Tag(CacheTags.TechApi.ProjectActivity)
        .Expire(TimeSpan.FromHours(1)));
});
builder.Services.AddCacheLibrary();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseOutputCache();

app.MapControllers();

app.Run();