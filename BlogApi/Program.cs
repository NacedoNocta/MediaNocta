using BlogApi.Interfaces;
using BlogApi.Services;
using BlogLibrairy.Interfaces;
using BlogAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddScoped<IBlogRepository, InMemoryBlogRepository>();
builder.Services.AddScoped<IAuthorRepository, InMemoryAuthorRepository>();
builder.Services.AddScoped<ITagRepository, InMemoryTagRepository>();
builder.Services.AddScoped<IBlogService, BlogService>();

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

app.MapControllers();

app.Run();
