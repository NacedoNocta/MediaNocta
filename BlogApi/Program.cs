using BlogApi.Interfaces;
using BlogApi.Services;
using BlogLibrary.Interfaces;
using BlogAPI.Repositories;
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

// Redis-backed output caching with named per-resource policies (24h TTL, tag-based eviction).
// Base policy honours the X-Skip-Cache bypass header (FR-D6).
// Fail-fast SE.Redis config so a Redis outage degrades to origin in <1s rather than ~10s Polly timeouts.
builder.AddRedisOutputCache("cache", configureOptions: CacheRedisDefaults.ApplyFailFast);
builder.Services.AddCacheBypass(CacheNamespaces.Main, CacheProducers.BlogApi);
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.AddPolicy<BypassHeaderPolicy>());

    var defaultTtl = TimeSpan.FromDays(1);
    options.AddPolicy("BlogList", b => b
        .Tag(CacheTags.BlogApi.List)
        .SetVaryByQuery("page", "pageSize")
        .Expire(defaultTtl));
    options.AddPolicy("BlogCount", b => b
        .Tag(CacheTags.BlogApi.Count)
        .Expire(defaultTtl));
    options.AddPolicy("BlogDetail", b => b
        .Tag(CacheTags.BlogApi.DetailAll)
        .SetVaryByRouteValue("id")
        .Expire(defaultTtl));
});
builder.Services.AddCacheLibrary();

// Add services to the container.
builder.Services.AddScoped<IBlogRepository, EfBlogRepository>();
builder.Services.AddScoped<IAuthorRepository, EfAuthorRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

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
