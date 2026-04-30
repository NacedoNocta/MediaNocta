using TechAPI.Interfaces;
using TechAPI.Services;
using TechAPI.Repositories;
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