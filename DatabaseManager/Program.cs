using DatabaseManager;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("mainDatabase");

builder.Services.AddHostedService<DatabaseMigrationWorker>();

var host = builder.Build();
host.Run();
