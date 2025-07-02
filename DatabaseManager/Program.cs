using DatabaseManager;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("PostgresDb");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("PostgreSQL connection string is missing from appsettings.json!");
}
Console.WriteLine($"✅ Connection String: {connectionString}");


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
    );

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
