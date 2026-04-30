using ActivityAPI.Repositories;
using CacheLibrary;
using DatabaseManager;
using DatabaseManager.DbContexts;
using SharedLibrary.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddNpgsqlDbContext<AppDbContext>("mainDatabase");

// Register repository
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();

// Redis-backed output caching with named per-resource policies (24h TTL, tag-based eviction).
// Base policy honours the X-Skip-Cache bypass header (FR-D6).
builder.AddRedisOutputCache("cache", configureOptions: CacheRedisDefaults.ApplyFailFast);
builder.Services.AddCacheBypass(CacheNamespaces.Main, CacheProducers.ActivityApi);
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.AddPolicy<BypassHeaderPolicy>());

    var defaultTtl = TimeSpan.FromDays(1);
    options.AddPolicy("ActivityRecent", b => b.Tag(CacheTags.ActivityApi.Recent).Expire(defaultTtl));
    options.AddPolicy("ActivityPinned", b => b.Tag(CacheTags.ActivityApi.Pinned).Expire(defaultTtl));
    options.AddPolicy("ActivityRandom", b => b.Tag(CacheTags.ActivityApi.Random).Expire(TimeSpan.FromMinutes(5)));
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
