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

// Auth database (backofficeDatabase) - used by Backoffice
// Note: BackofficeAuthMigrationWorker creates its own DbContext instance

// Register migration coordinator as singleton
builder.Services.AddSingleton(sp => new MigrationCoordinator(
    totalWorkers: 3,  // We have 3 migration workers
    applicationLifetime: sp.GetRequiredService<IHostApplicationLifetime>(),
    logger: sp.GetRequiredService<ILogger<MigrationCoordinator>>()
));

// Register all migration workers
builder.Services.AddHostedService<DatabaseMigrationWorker>();  // Content migrations
builder.Services.AddHostedService<AuthMigrationWorker>();       // Website auth migrations
builder.Services.AddHostedService<BackofficeAuthMigrationWorker>();  // Backoffice auth migrations

var host = builder.Build();
host.Run();
