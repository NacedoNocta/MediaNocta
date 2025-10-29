using DatabaseSeeder;
using DatabaseManager;
using DatabaseManager.DbContexts;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("mainDatabase");

builder.Services.AddScoped<SeedingService>();
builder.Services.AddHostedService<DatabaseSeederWorker>();

var host = builder.Build();
host.Run();