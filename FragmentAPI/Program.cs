using CacheLibrary;
using DatabaseManager;
using DatabaseManager.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults from the shared library
builder.AddServiceDefaults();

// Add database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("mainDatabase")));

// Redis-backed output caching with named per-resource policies (24h TTL, tag-based eviction).
// Base policy honours the X-Skip-Cache bypass header (FR-D6).
builder.AddRedisOutputCache("cache", configureOptions: CacheRedisDefaults.ApplyFailFast);
builder.Services.AddCacheBypass(CacheNamespaces.Main, CacheProducers.FragmentApi);
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.AddPolicy<BypassHeaderPolicy>());

    var defaultTtl = TimeSpan.FromDays(1);
    options.AddPolicy("FragmentList", b => b
        .Tag(CacheTags.FragmentApi.List)
        .SetVaryByQuery("page", "pageSize")
        .Expire(defaultTtl));
    options.AddPolicy("FragmentDetail", b => b
        .Tag(CacheTags.FragmentApi.DetailAll)
        .SetVaryByRouteValue("id")
        .Expire(defaultTtl));
    options.AddPolicy("FragmentTypes", b => b
        .Tag(CacheTags.FragmentApi.Types)
        .Expire(defaultTtl));
    options.AddPolicy("FragmentByType", b => b
        .Tag(CacheTags.FragmentApi.ByType)
        .SetVaryByRouteValue("typeTag")
        .Expire(defaultTtl));
    options.AddPolicy("FragmentSearch", b => b
        .Tag(CacheTags.FragmentApi.Search)
        .SetVaryByQuery("query", "typeTag")
        .Expire(TimeSpan.FromMinutes(15)));
});
builder.Services.AddCacheLibrary();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseOutputCache();

// Map default endpoints from service defaults
app.MapDefaultEndpoints();

// Map controllers
app.MapControllers();

app.Run();