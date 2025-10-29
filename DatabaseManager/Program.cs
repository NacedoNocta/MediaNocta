using DatabaseManager;
using DatabaseManager.DbContexts;
using DatabaseManager.Workers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Content database (mainDatabase) - used by APIs
builder.AddNpgsqlDbContext<AppDbContext>("mainDatabase");

// Auth database (websiteDatabase) - used by Website
builder.AddNpgsqlDbContext<AuthDbContext>("websiteDatabase");

// Register migration coordinator as singleton
builder.Services.AddSingleton(sp => new MigrationCoordinator(
    totalWorkers: 2,  // We have 2 migration workers
    applicationLifetime: sp.GetRequiredService<IHostApplicationLifetime>(),
    logger: sp.GetRequiredService<ILogger<MigrationCoordinator>>()
));

// Register both migration workers
builder.Services.AddHostedService<DatabaseMigrationWorker>();  // Content migrations
builder.Services.AddHostedService<AuthMigrationWorker>();       // Auth migrations

var host = builder.Build();
host.Run();
