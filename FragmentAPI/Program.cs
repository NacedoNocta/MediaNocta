using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using FragmentAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults from the shared library
builder.AddServiceDefaults();

// Add database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddScoped<FragmentSeedService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<FragmentSeedService>();
    await seedService.SeedFragmentTypesAsync();
    await seedService.SeedFragmentsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map default endpoints from service defaults
app.MapDefaultEndpoints();

// Map controllers
app.MapControllers();

app.Run();