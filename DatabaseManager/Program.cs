using DatabaseManager;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("medianocta");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
